// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.Feeder
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

#nullable disable
namespace Forza_EmuWheel;

internal class Feeder
{
    public static List<JoystickState> ControllerState { get; set; }

    public static int SteeringState { get; set; }

    private ControllerMapping GetOutputMapping()
    {
        ControllerMapping outputMapping = new ControllerMapping();
        InputCollector inputCollector = new InputCollector();
        int num = 128 /*0x80*/;
        outputMapping.Steering = inputCollector.GetAxisData("Steering");
        outputMapping.Combined = inputCollector.GetAxisData("Combined");
        outputMapping.Handbrake = inputCollector.GetAxisData("Handbrake");
        outputMapping.Throttle = inputCollector.GetAxisData("Throttle");
        outputMapping.Brake = inputCollector.GetAxisData("Brake");
        outputMapping.Clutch = inputCollector.GetAxisData("Clutch");
        List<ButtonData> source = new List<ButtonData>();
        for (int index = 0; index < num; ++index)
            source.AddRange((IEnumerable<ButtonData>)inputCollector.GetButtonData(index));
        outputMapping.Buttons = source.OrderBy<ButtonData, int>((Func<ButtonData, int>)(x => x.ButtonBind))
            .ToArray<ButtonData>();
        outputMapping.DPad = inputCollector.GetDPadData();
        return outputMapping;
    }

    public void PollControllers(List<Joystick> controllers, CancellationToken token)
    {
        ControllerMapping outputMapping = this.GetOutputMapping();
        this.ResetVJoyState();
        try
        {
            this.Poll(controllers, outputMapping, token);
        }
        finally
        {
            FFB.Stop();
            VJoyDevice.Joystick.RelinquishVJD(VJoyDevice.ID);
            foreach (var controller in controllers)
            {
                controller.Unacquire();
                controller.SetNotification(null);
            }
        }
    }

    private WaitHandle[] GetWaitHandles(List<Joystick> controllers, CancellationToken token)
    {
        var waitHandles = new List<WaitHandle>();
        foreach (var controller in controllers)
        {
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            controller.SetNotification(waitHandle);
            waitHandles.Add(waitHandle);
        }

        waitHandles.Add(token.WaitHandle);
        return waitHandles.ToArray();
    }

    private void Poll(List<Joystick> controllers, ControllerMapping mapping, CancellationToken token)
    {
        var waitHandles = this.GetWaitHandles(controllers, token);
        
        ControllerState = new List<JoystickState>();
        foreach (var controller in controllers)
        {
            controller.Acquire();
            ControllerState.Add(controller.GetCurrentState());
        }
        
        
        if (FFB.Start())
        {
            ConsoleMsg.Msg.Append("[SUCCESS] Force feedback is enabled.");
        }
        else
        {
            ConsoleMsg.Msg.Append("[INFO] Device does not appear to be FFB capable. FFB is disabled!");
        }

        this.FeedData(ControllerState, mapping);

        Feeder.SteeringState = -1;
        while (!token.IsCancellationRequested)
        {
            var index = WaitHandle.WaitAny(waitHandles);
            if (index >= controllers.Count)
            {
                // cancellation requested
                break;
            }

            var controller = controllers[index];
            try
            {
                controller.Poll();
                ControllerState[index] = controller.GetCurrentState();
                this.FeedData(Feeder.ControllerState, mapping);
            }
            catch
            {
                ConsoleMsg.Msg.Append(
                    $"[ERROR] Could not poll device '{controller.Information.InstanceName}'. Forza EmuWheel has stopped...");
                ConsoleMsg.Msg.StopIsEnabled = false;
                ConsoleMsg.Msg.StartIsEnabled = false;
                FFB.Stop();
                return;
            }
        }
    }

    private int GetAxisValue(JoystickState state, Axis.AxisEnum axisIndex)
    {
        return axisIndex switch
        {
            Axis.AxisEnum.X => state.X,
            Axis.AxisEnum.Y => state.Y,
            Axis.AxisEnum.Z => state.Z,
            Axis.AxisEnum.RotationX => state.RotationX,
            Axis.AxisEnum.RotationY => state.RotationY,
            Axis.AxisEnum.RotationZ => state.RotationZ,
            Axis.AxisEnum.Sliders0 => state.Sliders[0],
            Axis.AxisEnum.Sliders1 => state.Sliders[1],
            _ => throw new ArgumentOutOfRangeException(nameof(axisIndex), axisIndex, null)
        };
    }

    private void FeedData(List<JoystickState> data, ControllerMapping mapping)
    {
        if (mapping.Steering != null)
        {
            Axis.AxisEnum axisIndex = (Axis.AxisEnum)mapping.Steering.AxisIndex;
            int controllerIndex = mapping.Steering.ControllerIndex;
            var rawValue = this.GetAxisValue(data[controllerIndex], axisIndex);
            int value = Utils.DeadzoneAndInvert(rawValue, mapping.Steering, true);
            VJoyDevice.IReport.AxisX = (int)Math.Round((double)value / 2.0, 0);
           
            // For FFB
            // TODO (https://github.com/EmuWheel-RE/EmuWheel/issues/8): check this is correct
            SteeringState = rawValue;
        }

        if (mapping.Combined != null)
        {
            Axis.AxisEnum axisIndex = (Axis.AxisEnum)mapping.Combined.AxisIndex;
            int controllerIndex = mapping.Combined.ControllerIndex;
            int rawValue = this.GetAxisValue(data[controllerIndex], axisIndex);
            int inverted = mapping.Combined.Inverted;
            if (rawValue > 32768 /*0x8000*/)
            {
                int num = Utils.CombinedAxisSplit(rawValue, mapping.Combined, inverted);
                VJoyDevice.IReport.AxisXRot = (int)Math.Round((double)num / 2.0, 0);
                VJoyDevice.IReport.AxisZ = inverted != 1 ? (int)ushort.MaxValue : 0;
            }
            else if (rawValue < 32768 /*0x8000*/)
            {
                int value = Utils.CombinedAxisSplit(rawValue, mapping.Combined, inverted);
                VJoyDevice.IReport.AxisZ = (int)Math.Round((double)value / 2.0, 0);
                VJoyDevice.IReport.AxisXRot = inverted != 1 ? (int)ushort.MaxValue : 0;
            }
        }

        if (mapping.Throttle != null)
        {
            Axis.AxisEnum axisIndex = (Axis.AxisEnum)mapping.Throttle.AxisIndex;
            int controllerIndex = mapping.Throttle.ControllerIndex;
            int rawValue = this.GetAxisValue(data[controllerIndex], axisIndex);
            int value = Utils.DeadzoneAndInvert(rawValue, mapping.Throttle, false);
            VJoyDevice.IReport.AxisZ = (int)Math.Round((double)value / 2.0, 0);
        }

        if (mapping.Brake != null)
        {
            Axis.AxisEnum axisIndex = (Axis.AxisEnum)mapping.Brake.AxisIndex;
            int controllerIndex = mapping.Brake.ControllerIndex;
            int rawValue = this.GetAxisValue(data[controllerIndex], axisIndex);
            int value = Utils.DeadzoneAndInvert(rawValue, mapping.Brake, false);
            VJoyDevice.IReport.AxisXRot = (int)Math.Round((double)value / 2.0, 0);
        }

        if (mapping.Clutch != null)
        {
            Axis.AxisEnum axisIndex = (Axis.AxisEnum)mapping.Clutch.AxisIndex;
            int controllerIndex = mapping.Clutch.ControllerIndex;
            int rawValue = this.GetAxisValue(data[controllerIndex], axisIndex);
            int value = Utils.DeadzoneAndInvert(rawValue, mapping.Clutch, false);
            VJoyDevice.IReport.AxisYRot = (int)Math.Round((double)value / 2.0, 0);
        }

        if (mapping.Handbrake != null)
        {
            Axis.AxisEnum axisIndex = (Axis.AxisEnum)mapping.Handbrake.AxisIndex;
            int controllerIndex = mapping.Handbrake.ControllerIndex;
            int rawValue = this.GetAxisValue(data[controllerIndex], axisIndex);
            int value = Utils.DeadzoneAndInvert(rawValue, mapping.Handbrake, false);
            VJoyDevice.IReport.AxisZRot = (int)Math.Round((double)value / 2.0, 0);
        }

        if (mapping.Buttons != null)
        {
            bool[] buttonArray = this.GetButtonArray(data, mapping);
            VJoyDevice.IReport.Buttons = BitConverter.ToUInt32(Utils.PackBoolsInByteArray(buttonArray), 0);
        }

        if (mapping.DPad != null)
        {
            uint uint32 = Convert.ToUInt32(Utils.PackBoolsInByteArray(this.GetPOVArray(data, mapping))[0]);
            VJoyDevice.IReport.bHats = uint32;
        }

        if (VJoyDevice.Joystick.UpdateVJD(VJoyDevice.ID, ref VJoyDevice.IReport))
            return;
        VJoyDevice.Joystick.AcquireVJD(VJoyDevice.ID);
    }

    private bool[] GetPOVArray(List<JoystickState> data, ControllerMapping mapping)
    {
        bool[] povArray = new bool[4] { true, true, true, true };
        if (data[mapping.DPad.ControllerIndex].PointOfViewControllers[0] != -1)
        {
            povArray = new bool[4];
            if (data[mapping.DPad.ControllerIndex].PointOfViewControllers[0] == 9000)
                povArray[0] = true;
            else if (data[mapping.DPad.ControllerIndex].PointOfViewControllers[0] == 18000)
                povArray[1] = true;
            else if (data[mapping.DPad.ControllerIndex].PointOfViewControllers[0] == 27000)
            {
                povArray[0] = true;
                povArray[1] = true;
            }
        }

        return povArray;
    }

    private bool[] GetButtonArray(List<JoystickState> data, ControllerMapping mapping)
    {
        bool[] buttonArray = new bool[128 /*0x80*/];
        for (int index = 0; index < ((IEnumerable<ButtonData>)mapping.Buttons).Count<ButtonData>(); ++index)
        {
            if (!buttonArray[mapping.Buttons[index].ButtonBind])
                buttonArray[mapping.Buttons[index].ButtonBind] = data[mapping.Buttons[index].ControllerIndex]
                    .Buttons[mapping.Buttons[index].ButtonIndex];
        }

        return buttonArray;
    }

    private void ResetVJoyState()
    {
        VJoyDevice.IReport.bDevice = (byte)VJoyDevice.ID;
        VJoyDevice.IReport.AxisX = 16384 /*0x4000*/;
        VJoyDevice.IReport.AxisY = (int)short.MaxValue;
        VJoyDevice.IReport.AxisZ = (int)short.MaxValue;
        VJoyDevice.IReport.AxisXRot = (int)short.MaxValue;
        VJoyDevice.IReport.AxisYRot = (int)short.MaxValue;
        VJoyDevice.IReport.AxisZRot = (int)short.MaxValue;
        VJoyDevice.IReport.Slider = (int)short.MaxValue;
        VJoyDevice.IReport.Dial = (int)short.MaxValue;
        uint uint32_1 = BitConverter.ToUInt32(Utils.PackBoolsInByteArray(new bool[128 /*0x80*/]), 0);
        VJoyDevice.IReport.Buttons = uint32_1;
        uint uint32_2 =
            Convert.ToUInt32(Utils.PackBoolsInByteArray(Enumerable.Repeat<bool>(true, 4).ToArray<bool>())[0]);
        VJoyDevice.IReport.bHats = uint32_2;
        if (VJoyDevice.Joystick.UpdateVJD(VJoyDevice.ID, ref VJoyDevice.IReport))
            return;
        if (!VJoyDevice.Joystick.AcquireVJD(VJoyDevice.ID))
        {
            return;
        }

        VJoyDevice.Joystick.UpdateVJD(VJoyDevice.ID, ref VJoyDevice.IReport);
    }
}