// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.Feeder
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
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
            foreach (var controller in controllers)
                controller.Acquire();
            this.Poll(controllers, outputMapping, token);
        }
        finally
        {
            VJoyDevice.Joystick.RelinquishVJD(VJoyDevice.ID);
            foreach (var controller in controllers)
                controller.Unacquire();
        }
    }

    private void Poll(List<Joystick> controllers, ControllerMapping mapping, CancellationToken token)
    {
        Feeder.SteeringState = -1;
        while (!token.IsCancellationRequested)
        {
            Feeder.ControllerState = new List<JoystickState>();
            foreach (Joystick controller in controllers)
            {
                try
                {
                    controller.Poll();
                    Feeder.ControllerState.Add(controller.GetCurrentState());
                }
                catch
                {
                    ConsoleMsg msg = ConsoleMsg.Msg;
                    msg.Message =
                        $"{msg.Message}[ERROR] Could not poll device '{controller.Information.InstanceName}'. Forza EmuWheel has stopped...{Environment.NewLine}";
                    ConsoleMsg.Msg.StopIsEnabled = false;
                    ConsoleMsg.Msg.StartIsEnabled = false;
                    FFB.StopFFB = true;
                    return;
                }
            }

            this.FeedData(Feeder.ControllerState, mapping);
            Thread.Sleep(1);
        }
    }

    public void FeedData(List<JoystickState> data, ControllerMapping mapping)
    {
        if (mapping.Steering != null)
        {
            Axis.AxisEnum axisIndex = (Axis.AxisEnum)mapping.Steering.AxisIndex;
            int controllerIndex = mapping.Steering.ControllerIndex;
            int sliderValue = this.GetSliderValue(axisIndex, data, controllerIndex);
            if (sliderValue == -1)
            {
                sliderValue = (int)data[mapping.Steering.ControllerIndex].GetType().GetProperty(axisIndex.ToString())
                    .GetValue((object)data[mapping.Steering.ControllerIndex]);
                Feeder.SteeringState = sliderValue;
            }

            int num = Utils.DeadzoneAndInvert(sliderValue, mapping.Steering, true);
            VJoyDevice.IReport.AxisX = (int)Math.Round((double)num / 2.0, 0);
        }

        if (mapping.Combined != null)
        {
            Axis.AxisEnum axisIndex = (Axis.AxisEnum)mapping.Combined.AxisIndex;
            int controllerIndex = mapping.Combined.ControllerIndex;
            int sliderValue = this.GetSliderValue(axisIndex, data, controllerIndex);
            if (sliderValue == -1)
                sliderValue = (int)data[mapping.Combined.ControllerIndex].GetType().GetProperty(axisIndex.ToString())
                    .GetValue((object)data[mapping.Combined.ControllerIndex]);
            int inverted = mapping.Combined.Inverted;
            if (sliderValue > 32768 /*0x8000*/)
            {
                int num = Utils.CombinedAxisSplit(sliderValue, mapping.Combined, inverted);
                VJoyDevice.IReport.AxisXRot = (int)Math.Round((double)num / 2.0, 0);
                VJoyDevice.IReport.AxisZ = inverted != 1 ? (int)ushort.MaxValue : 0;
            }
            else if (sliderValue < 32768 /*0x8000*/)
            {
                int num = Utils.CombinedAxisSplit(sliderValue, mapping.Combined, inverted);
                VJoyDevice.IReport.AxisZ = (int)Math.Round((double)num / 2.0, 0);
                VJoyDevice.IReport.AxisXRot = inverted != 1 ? (int)ushort.MaxValue : 0;
            }
        }

        if (mapping.Throttle != null)
        {
            Axis.AxisEnum axisIndex = (Axis.AxisEnum)mapping.Throttle.AxisIndex;
            int controllerIndex = mapping.Throttle.ControllerIndex;
            int sliderValue = this.GetSliderValue(axisIndex, data, controllerIndex);
            if (sliderValue == -1)
                sliderValue = (int)data[mapping.Throttle.ControllerIndex].GetType().GetProperty(axisIndex.ToString())
                    .GetValue((object)data[mapping.Throttle.ControllerIndex]);
            int num = Utils.DeadzoneAndInvert(sliderValue, mapping.Throttle, false);
            VJoyDevice.IReport.AxisZ = (int)Math.Round((double)num / 2.0, 0);
        }

        if (mapping.Brake != null)
        {
            Axis.AxisEnum axisIndex = (Axis.AxisEnum)mapping.Brake.AxisIndex;
            int controllerIndex = mapping.Brake.ControllerIndex;
            int sliderValue = this.GetSliderValue(axisIndex, data, controllerIndex);
            if (sliderValue == -1)
                sliderValue = (int)data[mapping.Brake.ControllerIndex].GetType().GetProperty(axisIndex.ToString())
                    .GetValue((object)data[mapping.Brake.ControllerIndex]);
            int num = Utils.DeadzoneAndInvert(sliderValue, mapping.Brake, false);
            VJoyDevice.IReport.AxisXRot = (int)Math.Round((double)num / 2.0, 0);
        }

        if (mapping.Clutch != null)
        {
            Axis.AxisEnum axisIndex = (Axis.AxisEnum)mapping.Clutch.AxisIndex;
            int controllerIndex = mapping.Clutch.ControllerIndex;
            int sliderValue = this.GetSliderValue(axisIndex, data, controllerIndex);
            if (sliderValue == -1)
                sliderValue = (int)data[mapping.Clutch.ControllerIndex].GetType().GetProperty(axisIndex.ToString())
                    .GetValue((object)data[mapping.Clutch.ControllerIndex]);
            int num = Utils.DeadzoneAndInvert(sliderValue, mapping.Clutch, false);
            VJoyDevice.IReport.AxisYRot = (int)Math.Round((double)num / 2.0, 0);
        }

        if (mapping.Handbrake != null)
        {
            Axis.AxisEnum axisIndex = (Axis.AxisEnum)mapping.Handbrake.AxisIndex;
            int controllerIndex = mapping.Handbrake.ControllerIndex;
            int sliderValue = this.GetSliderValue(axisIndex, data, controllerIndex);
            if (sliderValue == -1)
                sliderValue = (int)data[mapping.Handbrake.ControllerIndex].GetType().GetProperty(axisIndex.ToString())
                    .GetValue((object)data[mapping.Handbrake.ControllerIndex]);
            int num = Utils.DeadzoneAndInvert(sliderValue, mapping.Handbrake, false);
            VJoyDevice.IReport.AxisZRot = (int)Math.Round((double)num / 2.0, 0);
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

    private int GetSliderValue(Axis.AxisEnum axis, List<JoystickState> data, int index)
    {
        int sliderValue = -1;
        if (axis.ToString().IndexOf("Sliders") == -1)
            return sliderValue;
        int[] numArray = (int[])data[index].GetType().GetProperty(axis.ToString().Remove(7))
            .GetValue((object)data[index]);
        if (axis.ToString().IndexOf("0") != -1)
            sliderValue = numArray[0];
        else if (axis.ToString().IndexOf("1") != -1)
            sliderValue = numArray[1];
        return sliderValue;
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