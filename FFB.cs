// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.FFB
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using vJoyInterfaceWrap;

#nullable disable
namespace Forza_EmuWheel;

internal class FFB
{
    public static bool StopFFB = false;
    private FFBPacketHandler PacketHandler;

    private static Dictionary<string, Guid> EffectDictionary = new Dictionary<string, Guid>()
    {
        {
            "ET_CONST",
            EffectGuid.ConstantForce
        },
        {
            "ET_SINE",
            EffectGuid.Sine
        },
        {
            "ET_SPRNG",
            EffectGuid.Spring
        },
        {
            "ET_DMPR",
            EffectGuid.Damper
        }
    };

    public static Joystick FFBDevice { get; set; }

    public FFB(Joystick device) => FFB.FFBDevice = device;

    public event FFB.FFBDataReceiveEventHandler FFBDataReceived;

    private void OnFFBDataReceived(FFBEventArgs e)
    {
        FFB.FFBDataReceiveEventHandler ffbDataReceived = this.FFBDataReceived;
        if (ffbDataReceived == null)
            return;
        ffbDataReceived(this, e);
    }

    public void OnVirtualFFBDataReceived(IntPtr data, object userData)
    {
        if (FFB.StopFFB)
            return;
        this.PacketHandler.ProcessFFBPacket(data, userData, new Action<FFBEventArgs>(this.OnFFBDataReceived),
            FFB.FFBDevice);
    }

    public static List<int> ActuatorsObjectTypes { get; set; }

    public bool StartFFB(vJoy joystick)
    {
        if (!FFB.LoadEffects())
            return false;
        this.PacketHandler = new FFBPacketHandler(joystick);
        object data = new object();
        joystick.FfbRegisterGenCB(new vJoy.FfbCbFunc(this.OnVirtualFFBDataReceived), data);
        return true;
    }

    private static float ConstantMagnitudeMulti { get; set; }

    private static float ConstantGainMulti { get; set; }

    private static float SineGainMulti { get; set; }

    private static float SineFrequencyMulti { get; set; }

    private static float EngineVibrationFrequency { get; set; }

    private static float EngineVibrationStrength { get; set; }

    private static float GearShiftVibrationFrequency { get; set; }

    private static float GearShiftVibrationStrength { get; set; }

    private static float SineWavePhase { get; set; }

    private static float SineWaveMagnitude { get; set; }

    private static float SpringCoefficient { get; set; }

    private static float DamperCoefficient { get; set; }

    private static float SpringSaturation { get; set; }

    private static float DamperSaturation { get; set; }

    private static float ConstMinimumForce { get; set; }

    private static float SineMinimumForce { get; set; }

    private static float FilterThreshold { get; set; }

    private static float MinimumCoefficient { get; set; }

    private static void LoadSettings()
    {
        try
        {
            Controller controller = InputCollector.Controllers
                .Where<Controller>(x =>
                    x.InstanceGuid == FFB.FFBDevice.Information.InstanceGuid)
                .Select<Controller, Controller>(x => x).First<Controller>();
            FFB.ConstantMagnitudeMulti = controller.FFBParameters.Const.Magnitude;
            FFB.ConstantGainMulti = controller.FFBParameters.Const.MaximumForce;
            FFB.SineFrequencyMulti = controller.FFBParameters.Sine.Frequency;
            FFB.SineGainMulti = controller.FFBParameters.Sine.MaximumForce;
            FFB.EngineVibrationFrequency = controller.FFBParameters.Sine.EngineVibrations.Frequency;
            FFB.EngineVibrationStrength = controller.FFBParameters.Sine.EngineVibrations.Strength;
            FFB.GearShiftVibrationFrequency = controller.FFBParameters.Sine.GearShiftVibrations.Frequency;
            FFB.GearShiftVibrationStrength = controller.FFBParameters.Sine.GearShiftVibrations.Strength;
            FFB.SineWavePhase = controller.FFBParameters.Sine.Phase;
            FFB.SineWaveMagnitude = controller.FFBParameters.Sine.Magnitude;
            FFB.SpringCoefficient = controller.FFBParameters.Spring.Coefficient;
            FFB.DamperCoefficient = controller.FFBParameters.Damper.Coefficient;
            FFB.SpringSaturation = controller.FFBParameters.Spring.Saturation;
            FFB.DamperSaturation = controller.FFBParameters.Damper.Saturation;
            FFB.ConstMinimumForce = controller.FFBParameters.Const.MinimumForce;
            FFB.SineMinimumForce = controller.FFBParameters.Sine.MinimumForce;
            FFB.FilterThreshold = controller.FFBParameters.Const.FilterThreshold;
            FFB.MinimumCoefficient = controller.FFBParameters.Const.MinimumCoefficient;
        }
        catch
        {
            ConsoleMsg msg = ConsoleMsg.Msg;
            msg.Message = $"{msg.Message}[ERROR] Could not load FFB settings.{Environment.NewLine}";
        }
    }

    private static Device FFBDev { get; set; }

    private static Effect ConstantEffect { get; set; }

    private static Effect SineEffect { get; set; }

    private static Effect SpringEffect { get; set; }

    private static Effect DamperEffect { get; set; }

    private static bool LoadEffects()
    {
        FFB.LoadSettings();
        try
        {
            FFB.FFBDev = new Device(FFB.FFBDevice.NativePointer);
            FFB.FFBDev.Properties.AutoCenter = false;
            FFB.FFBDev.Acquire();
            EffectParameters parameters = new EffectParameters();
            int[] axes = new int[1] { FFB.ActuatorsObjectTypes[0] };
            int[] directions = new int[1]
            {
                FFBPacketHandler.EffectReport.Direction
            };
            parameters.Duration = -1;
            parameters.Flags = EffectFlags.ObjectIds | EffectFlags.Cartesian;
            parameters.Gain = 10000;
            parameters.SetAxes(axes, directions);
            parameters.StartDelay = 0;
            parameters.SamplePeriod = 0;
            parameters.TriggerButton = -1;
            parameters.TriggerRepeatInterval = 0;
            TypeSpecificParameters specificParameters1 = new ConstantForce();
            TypeSpecificParameters specificParameters2 = new PeriodicForce();
            TypeSpecificParameters specificParameters3 = new ConditionSet();
            specificParameters3.As<ConditionSet>().Conditions = new SharpDX.DirectInput.Condition[1];
            parameters.Parameters = specificParameters1;
            FFB.ConstantEffect = new Effect(FFB.FFBDev, EffectGuid.ConstantForce, parameters);
            parameters.Parameters = specificParameters2;
            FFB.SineEffect = new Effect(FFB.FFBDev, EffectGuid.Sine, parameters);
            parameters.Parameters = specificParameters3;
            FFB.SpringEffect = new Effect(FFB.FFBDev, EffectGuid.Spring, parameters);
            FFB.DamperEffect = new Effect(FFB.FFBDev, EffectGuid.Damper, parameters);
            return true;
        }
        catch
        {
            ConsoleMsg msg = ConsoleMsg.Msg;
            msg.Message =
                $"{msg.Message}[ERROR] Could not load Force Feedback effects in memory of  '{FFB.FFBDevice.Information.InstanceName}' {Environment.NewLine}";
            return false;
        }
    }

    private static void SendConstant(Joystick FFBDevice)
    {
        TypeSpecificParameters specificParameters = new ConstantForce();
        int num1 = (int)Math.Round(
            FFBPacketHandler.ConstantForceReport.Magnitude * (double)FFB.ConstantMagnitudeMulti, 0);
        int num2 = (int)Math.Round(FFB.ConstantGainMulti * 10000.0, 0);
        if (Math.Abs(num1) > num2)
            num1 = num2 * Math.Sign(num1);
        if (Math.Abs(num1) < (int)(FFB.ConstMinimumForce * 10000.0))
            num1 = (int)(FFB.ConstMinimumForce * 10000.0) * Math.Sign(num1);
        if (Math.Abs(num1) > 10000)
            num1 = 10000 * Math.Sign(num1);
        if (Math.Abs(num1) > FFB.FilterThreshold * 10000.0 && Feeder.SteeringState != -1)
        {
            int steeringState = Feeder.SteeringState;
            if (steeringState < short.MaxValue)
            {
                float num3 = 1f - steeringState / (float)short.MaxValue;
                if (num3 < (double)FFB.MinimumCoefficient)
                    num3 = FFB.MinimumCoefficient;
                num1 = (int)(num1 * (double)num3);
            }
            else if (steeringState > short.MaxValue)
            {
                float num4 = (steeringState - short.MaxValue) / (float)short.MaxValue;
                if (num4 < (double)FFB.MinimumCoefficient)
                    num4 = FFB.MinimumCoefficient;
                num1 = (int)(num1 * (double)num4);
            }
            else
                num1 = (int)(num1 * (double)FFB.MinimumCoefficient);
        }

        specificParameters.As<ConstantForce>().Magnitude = -num1;
        EffectParameters effectParameters = new EffectParameters()
        {
            Parameters = specificParameters
        };
        int[] axes = new int[1] { FFB.ActuatorsObjectTypes[0] };
        int[] directions = new int[1]
        {
            FFBPacketHandler.EffectReport.Direction
        };
        effectParameters.Duration = FFBPacketHandler.EffectReport.Duration * 1000;
        if (FFBPacketHandler.EffectReport.Duration == ushort.MaxValue ||
            FFBPacketHandler.EffectReport.Duration == 0)
            effectParameters.Duration = -1;
        effectParameters.Flags = EffectFlags.ObjectIds | EffectFlags.Cartesian;
        effectParameters.Gain =
            (int)Math.Round(FFBPacketHandler.EffectReport.Gain / (double)byte.MaxValue * 10000.0, 0);
        effectParameters.SetAxes(axes, directions);
        effectParameters.StartDelay = 0;
        effectParameters.SamplePeriod = FFBPacketHandler.EffectReport.SamplePrd;
        effectParameters.TriggerButton = FFBPacketHandler.EffectReport.TrigerBtn;
        if (FFBPacketHandler.EffectReport.TrigerBtn == byte.MaxValue)
            effectParameters.TriggerButton = -1;
        effectParameters.TriggerRepeatInterval = FFBPacketHandler.EffectReport.TrigerRpt;
        FFB.ConstantEffect.SetParameters(effectParameters, EffectParameterFlags.TypeSpecificParameters);
        if (FFB.ConstantEffect.Status != EffectStatus.Playing)
            FFB.ConstantEffect.Start(1, EffectPlayFlags.NoDownload);
        FFBPacketHandler.Op = new vJoy.FFB_EFF_OP();
    }

    private static bool SineBug { get; set; }

    private static byte SineGainValue { get; set; }

    private static bool SineEngineBug { get; set; }

    private static bool SineGearShiftBug { get; set; }

    private static void SendPeriodic(Joystick FFBDevice)
    {
        TypeSpecificParameters specificParameters = new PeriodicForce();
        specificParameters.As<PeriodicForce>().Magnitude = (int)FFBPacketHandler.PeriodicReport.Magnitude;
        specificParameters.As<PeriodicForce>().Offset = FFBPacketHandler.PeriodicReport.Offset;
        specificParameters.As<PeriodicForce>().Period =
            (int)Math.Round(FFBPacketHandler.PeriodicReport.Period * 1000U / (double)FFB.SineFrequencyMulti,
                0);
        specificParameters.As<PeriodicForce>().Phase = (int)(FFB.SineWavePhase * 35999.0);
        EffectParameters effectParameters = new EffectParameters()
        {
            Parameters = specificParameters
        };
        FFB.EffectDictionary
            .Where<KeyValuePair<string, Guid>>(x =>
                x.Key == FFBPacketHandler.EffectReport.EffectType.ToString())
            .Select<KeyValuePair<string, Guid>, Guid>(x => x.Value)
            .First<Guid>();
        int[] axes = new int[1] { FFB.ActuatorsObjectTypes[0] };
        int[] directions = new int[1]
        {
            FFBPacketHandler.EffectReport.Direction
        };
        effectParameters.Duration = FFBPacketHandler.EffectReport.Duration * 1000;
        if (FFBPacketHandler.EffectReport.Duration == ushort.MaxValue ||
            FFBPacketHandler.EffectReport.Duration == 0)
            effectParameters.Duration = -1;
        effectParameters.Flags = EffectFlags.ObjectIds | EffectFlags.Cartesian;
        int num1 = (int)Math.Round(
            FFBPacketHandler.EffectReport.Gain / (double)byte.MaxValue * 10000.0 *
            FFB.SineWaveMagnitude, 0);
        int num2 = (int)Math.Round(FFB.SineGainMulti * 10000.0, 0);
        if (num1 > num2)
            num1 = num2;
        if (num1 < (int)Math.Round(FFB.SineMinimumForce * 10000.0, 0) && num1 > 0)
            num1 = (int)Math.Round(FFB.SineMinimumForce * 10000.0, 0);
        if (FFBPacketHandler.EffectReport.Gain == 32 /*0x20*/)
        {
            FFB.SineGearShiftBug = true;
            num1 = (int)Math.Round(FFB.GearShiftVibrationStrength * 10000.0, 0);
            specificParameters.As<PeriodicForce>().Period = (int)Math.Round(
                FFBPacketHandler.PeriodicReport.Period * 1000U / (double)FFB.GearShiftVibrationFrequency, 0);
        }
        else if (FFB.SineGearShiftBug && FFBPacketHandler.EffectReport.Gain == 0)
        {
            num1 = (int)Math.Round(FFB.GearShiftVibrationStrength * 10000.0, 0);
            specificParameters.As<PeriodicForce>().Period = (int)Math.Round(
                FFBPacketHandler.PeriodicReport.Period * 1000U / (double)FFB.GearShiftVibrationFrequency, 0);
            FFB.SineGearShiftBug = false;
        }

        if (FFBPacketHandler.EffectReport.Gain == 1)
        {
            FFB.SineEngineBug = true;
            num1 = (int)Math.Round(FFB.EngineVibrationStrength * 10000.0, 0);
            specificParameters.As<PeriodicForce>().Period = (int)Math.Round(
                FFBPacketHandler.PeriodicReport.Period * 1000U / (double)FFB.EngineVibrationFrequency, 0);
        }
        else if (FFB.SineEngineBug && FFBPacketHandler.EffectReport.Gain == 0)
        {
            num1 = (int)Math.Round(FFB.EngineVibrationStrength * 10000.0, 0);
            specificParameters.As<PeriodicForce>().Period = (int)Math.Round(
                FFBPacketHandler.PeriodicReport.Period * 1000U / (double)FFB.EngineVibrationFrequency, 0);
            FFB.SineEngineBug = false;
        }

        if (num1 > 10000)
            num1 = 10000;
        specificParameters.As<PeriodicForce>().Magnitude = num1;
        effectParameters.Parameters = specificParameters;
        effectParameters.SetAxes(axes, directions);
        effectParameters.StartDelay = 0;
        effectParameters.SamplePeriod = FFBPacketHandler.EffectReport.SamplePrd;
        effectParameters.TriggerButton = FFBPacketHandler.EffectReport.TrigerBtn;
        if (FFBPacketHandler.EffectReport.TrigerBtn == byte.MaxValue)
            effectParameters.TriggerButton = -1;
        effectParameters.TriggerRepeatInterval = FFBPacketHandler.EffectReport.TrigerRpt;
        FFB.SineEffect.SetParameters(effectParameters, EffectParameterFlags.TypeSpecificParameters);
        if (FFB.SineEffect.Status != EffectStatus.Playing)
            FFB.SineEffect.Start();
        FFBPacketHandler.Op = new vJoy.FFB_EFF_OP();
    }

    private static void SendCondition(Joystick FFBDevice)
    {
        Guid effectGuid = new Guid();
        TypeSpecificParameters specificParameters = new ConditionSet();
        specificParameters.As<ConditionSet>().Conditions = new SharpDX.DirectInput.Condition[1];
        specificParameters.As<ConditionSet>().Conditions[0].DeadBand = FFBPacketHandler.ConditionalReport.DeadBand;
        specificParameters.As<ConditionSet>().Conditions[0].Offset =
            FFBPacketHandler.ConditionalReport.CenterPointOffset;
        float num1 = 1f;
        float num2 = 1f;
        if (FFBPacketHandler.EffectReport.EffectType.ToString() == "ET_DMPR")
        {
            num1 = FFB.DamperCoefficient;
            num2 = FFB.DamperSaturation;
        }
        else if (FFBPacketHandler.EffectReport.EffectType.ToString() == "ET_SPRNG")
        {
            num1 = FFB.SpringCoefficient;
            num2 = FFB.SpringSaturation;
        }

        int num3 = (int)Math.Round(FFBPacketHandler.ConditionalReport.PosCoeff * (double)num1, 0);
        int num4 = (int)Math.Round(FFBPacketHandler.ConditionalReport.NegCoeff * (double)num1, 0);
        int num5 = (int)Math.Round(FFBPacketHandler.ConditionalReport.PosSatur * (double)num2, 0);
        int num6 = (int)Math.Round(FFBPacketHandler.ConditionalReport.NegSatur * (double)num2, 0);
        if (num5 > 10000)
            num5 = 10000;
        if (num6 > 10000)
            num6 = 10000;
        specificParameters.As<ConditionSet>().Conditions[0].PositiveCoefficient = num3;
        specificParameters.As<ConditionSet>().Conditions[0].NegativeCoefficient = num4;
        specificParameters.As<ConditionSet>().Conditions[0].PositiveSaturation = num5;
        specificParameters.As<ConditionSet>().Conditions[0].NegativeSaturation = num6;
        EffectParameters effectParameters = new EffectParameters()
        {
            Parameters = specificParameters
        };
        effectGuid = FFB.EffectDictionary
            .Where<KeyValuePair<string, Guid>>(x =>
                x.Key == FFBPacketHandler.EffectReport.EffectType.ToString())
            .Select<KeyValuePair<string, Guid>, Guid>(x => x.Value)
            .First<Guid>();
        Effect effect = new Effect(FFBDevice.CreatedEffects
            .Where<Effect>(x => x.Guid == effectGuid)
            .Select<Effect, IntPtr>(x => x.NativePointer).First<IntPtr>());
        int[] axes = new int[1] { FFB.ActuatorsObjectTypes[0] };
        int[] directions = new int[1]
        {
            FFBPacketHandler.EffectReport.Direction
        };
        effectParameters.Duration = FFBPacketHandler.EffectReport.Duration * 1000;
        if (FFBPacketHandler.EffectReport.Duration == ushort.MaxValue ||
            FFBPacketHandler.EffectReport.Duration == 0)
            effectParameters.Duration = -1;
        effectParameters.Flags = EffectFlags.ObjectIds | EffectFlags.Cartesian;
        effectParameters.Gain =
            (int)Math.Round(FFBPacketHandler.EffectReport.Gain / (double)byte.MaxValue * 10000.0, 0);
        effectParameters.SetAxes(axes, directions);
        effectParameters.StartDelay = 0;
        effectParameters.SamplePeriod = FFBPacketHandler.EffectReport.SamplePrd;
        effectParameters.TriggerButton = FFBPacketHandler.EffectReport.TrigerBtn;
        if (FFBPacketHandler.EffectReport.TrigerBtn == byte.MaxValue)
            effectParameters.TriggerButton = -1;
        effectParameters.TriggerRepeatInterval = FFBPacketHandler.EffectReport.TrigerRpt;
        if (FFBPacketHandler.EffectReport.EffectType.ToString() == "ET_DMPR")
        {
            FFB.DamperEffect.SetParameters(effectParameters, EffectParameterFlags.TypeSpecificParameters);
            if (FFB.DamperEffect.Status != EffectStatus.Playing)
                FFB.DamperEffect.Start();
        }
        else if (FFBPacketHandler.EffectReport.EffectType.ToString() == "ET_SPRNG")
        {
            FFB.SpringEffect.SetParameters(effectParameters, EffectParameterFlags.TypeSpecificParameters);
            if (FFB.SpringEffect.Status != EffectStatus.Playing)
                FFB.SpringEffect.Start();
        }

        FFBPacketHandler.Op = new vJoy.FFB_EFF_OP();
    }

    public static void SendFFBData(Joystick FFBDevice)
    {
        switch (FFBPacketHandler.PacketType)
        {
            case FFBPType.PT_CTRLREP:
                SendFFBControlData(FFBDevice);
                return;
            case FFBPType.PT_EFOPREP:
            case FFBPType.PT_EFFREP:
                SendFFBEffectData(FFBDevice);
                return;
        }
    }

    private static void SendFFBControlData(Joystick FFBDevice)
    {
        switch (FFBPacketHandler.Control)
        {
            case FFB_CTRL.CTRL_DEVRST:
                FFBDevice.SendForceFeedbackCommand(ForceFeedbackCommand.Reset);
                return;
            case FFB_CTRL.CTRL_STOPALL:
            case FFB_CTRL.CTRL_DISACT:
                FFBDevice.SendForceFeedbackCommand(ForceFeedbackCommand.StopAll);
                return;
            case FFB_CTRL.CTRL_DEVPAUSE:
                FFBDevice.SendForceFeedbackCommand(ForceFeedbackCommand.Pause);
                return;
            case FFB_CTRL.CTRL_DEVCONT:
                FFBDevice.SendForceFeedbackCommand(ForceFeedbackCommand.Continue);
                return;
        }
    }

    private static void SendFFBEffectData(Joystick FFBDevice)
    {
        Debug.Assert(FFBPacketHandler.PacketType == FFBPType.PT_EFOPREP || FFBPacketHandler.PacketType == FFBPType.PT_EFFREP);

        if (FFBPacketHandler.EffectReport.EffectType == FFBEType.ET_NONE)
        {
            return;
        }

        if (FFBPacketHandler.Op.EffectOp == FFBOP.EFF_STOP)
        {
            switch (FFBPacketHandler.EffectReport.EffectType)
            {
                case FFBEType.ET_CONST:
                    FFB.ConstantEffect.Stop();
                    break;
                case FFBEType.ET_SINE:
                    FFB.SineEffect.Stop();
                    break;
                case FFBEType.ET_DMPR:
                    FFB.DamperEffect.Stop();
                    break;
                case FFBEType.ET_SPRNG:
                    FFB.SpringEffect.Stop();
                    break;
            }
        }

        switch (FFBPacketHandler.EffectReport.EffectType)
        {
            case FFBEType.ET_SINE:
                FFB.SendPeriodic(FFBDevice);
                break;
            case FFBEType.ET_CONST:
                FFB.SendConstant(FFBDevice);
                break;
            case FFBEType.ET_DMPR:
            case FFBEType.ET_SPRNG:
                FFB.SendCondition(FFBDevice);
                break;
        }
    }

    public delegate void FFBDataReceiveEventHandler(object sender, EventArgs e);
}