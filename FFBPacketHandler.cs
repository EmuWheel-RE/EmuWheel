// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.FFBPacketHandler
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

using System;
using vJoyInterfaceWrap;

#nullable disable
namespace Forza_EmuWheel;

internal class FFBPacketHandler
{
    private vJoy Joystick;
    private const uint ERROR_SUCCESS = 0;
    private const uint ERROR_INVALID_PARAMETER = 87;
    private const uint ERROR_INVALID_DATA = 13;
    public static FFBData FFBDataUnit = new FFBData();
    public static vJoy.FFB_EFF_REPORT OldEffectReport = new vJoy.FFB_EFF_REPORT();
    public static vJoy.FFB_EFF_COND OldConditionalReport = new vJoy.FFB_EFF_COND();
    public static vJoy.FFB_EFF_PERIOD OldPeriodicReport = new vJoy.FFB_EFF_PERIOD();
    public static vJoy.FFB_EFF_CONSTANT OldConstantForceReport = new vJoy.FFB_EFF_CONSTANT();
    public static vJoy.FFB_EFF_ENVLP OldEnvelopeReport = new vJoy.FFB_EFF_ENVLP();
    public static vJoy.FFB_EFF_RAMP OldRampForceReport = new vJoy.FFB_EFF_RAMP();
    public static vJoy.FFB_EFF_OP OldOp = new vJoy.FFB_EFF_OP();
    public static byte OldGain = 0;
    public static vJoy.FFB_EFF_REPORT EffectReport = new vJoy.FFB_EFF_REPORT();
    public static vJoy.FFB_EFF_ENVLP EnvelopeReport = new vJoy.FFB_EFF_ENVLP();
    public static vJoy.FFB_EFF_COND ConditionalReport = new vJoy.FFB_EFF_COND();
    public static vJoy.FFB_EFF_PERIOD PeriodicReport = new vJoy.FFB_EFF_PERIOD();
    public static vJoy.FFB_EFF_CONSTANT ConstantForceReport = new vJoy.FFB_EFF_CONSTANT();
    public static vJoy.FFB_EFF_RAMP RampForceReport = new vJoy.FFB_EFF_RAMP();
    public static vJoy.FFB_EFF_OP Op = new vJoy.FFB_EFF_OP();
    public static FFB_CTRL Control = (FFB_CTRL)0;
    public static byte Gain = 0;
    public static FFBPType PacketType = (FFBPType)0;
    public static FFBPType NewPacketType = (FFBPType)0;

    public FFBPacketHandler(vJoy joystick) => this.Joystick = joystick;

    public void ProcessFFBPacket(
        IntPtr data,
        object userData,
        Action<FFBEventArgs> callback,
        SharpDX.DirectInput.Joystick FFBDevice)
    {
        FFBEventArgs ffbEventArgs = new FFBEventArgs();
        int num = (int)this.Joystick.Ffb_h_Type(data, ref FFBPacketHandler.PacketType);
        switch (FFBPacketHandler.PacketType.ToString())
        {
            case "PT_CONDREP":
                if (this.Joystick.Ffb_h_Eff_Cond(data, ref FFBPacketHandler.ConditionalReport) == 0U)
                {
                    FFBPacketHandler.FFBDataUnit.ConditionalReport = FFBPacketHandler.ConditionalReport;
                    if (Utils.CompareObjects((object)FFBPacketHandler.OldConditionalReport,
                            (object)FFBPacketHandler.ConditionalReport))
                        return;
                    FFBPacketHandler.OldConditionalReport = FFBPacketHandler.ConditionalReport;
                    break;
                }

                break;
            case "PT_CONSTREP":
                if (this.Joystick.Ffb_h_Eff_Constant(data, ref FFBPacketHandler.ConstantForceReport) == 0U)
                {
                    FFBPacketHandler.FFBDataUnit.ConstantForceReport = FFBPacketHandler.ConstantForceReport;
                    if (Utils.CompareObjects((object)FFBPacketHandler.OldConstantForceReport,
                            (object)FFBPacketHandler.ConstantForceReport))
                        return;
                    FFBPacketHandler.OldConstantForceReport = FFBPacketHandler.ConstantForceReport;
                    break;
                }

                break;
            case "PT_CTRLREP":
                if (this.Joystick.Ffb_h_DevCtrl(data, ref FFBPacketHandler.Control) == 0U)
                {
                    FFBPacketHandler.FFBDataUnit.Control = FFBPacketHandler.Control;
                    break;
                }

                break;
            case "PT_EFFREP":
                if (this.Joystick.Ffb_h_Eff_Report(data, ref FFBPacketHandler.EffectReport) == 0U)
                {
                    FFBPacketHandler.FFBDataUnit.EffectReport = FFBPacketHandler.EffectReport;
                    if (Utils.CompareObjects((object)FFBPacketHandler.OldEffectReport,
                            (object)FFBPacketHandler.EffectReport))
                        return;
                    FFBPacketHandler.OldEffectReport = FFBPacketHandler.EffectReport;
                    break;
                }

                break;
            case "PT_EFOPREP":
                if (this.Joystick.Ffb_h_EffOp(data, ref FFBPacketHandler.Op) == 0U)
                {
                    FFBPacketHandler.OldOp = FFBPacketHandler.Op;
                    FFBPacketHandler.FFBDataUnit.Op = FFBPacketHandler.Op;
                    break;
                }

                break;
            case "PT_ENVREP":
                if (this.Joystick.Ffb_h_Eff_Envlp(data, ref FFBPacketHandler.EnvelopeReport) == 0U)
                {
                    FFBPacketHandler.FFBDataUnit.EnvelopeReport = FFBPacketHandler.EnvelopeReport;
                    if (Utils.CompareObjects((object)FFBPacketHandler.OldEnvelopeReport,
                            (object)FFBPacketHandler.EnvelopeReport))
                        return;
                    FFBPacketHandler.OldEnvelopeReport = FFBPacketHandler.EnvelopeReport;
                    break;
                }

                break;
            case "PT_GAINREP":
                if (this.Joystick.Ffb_h_DevGain(data, ref FFBPacketHandler.Gain) == 0U)
                {
                    FFBPacketHandler.FFBDataUnit.Gain = FFBPacketHandler.Gain;
                    if ((int)FFBPacketHandler.OldGain == (int)FFBPacketHandler.Gain)
                        return;
                    FFBPacketHandler.OldGain = FFBPacketHandler.Gain;
                    break;
                }

                break;
            case "PT_PRIDREP":
                if (this.Joystick.Ffb_h_Eff_Period(data, ref FFBPacketHandler.PeriodicReport) == 0U)
                {
                    FFBPacketHandler.FFBDataUnit.PeriodicReport = FFBPacketHandler.PeriodicReport;
                    if (Utils.CompareObjects((object)FFBPacketHandler.OldPeriodicReport,
                            (object)FFBPacketHandler.PeriodicReport))
                        return;
                    FFBPacketHandler.OldPeriodicReport = FFBPacketHandler.PeriodicReport;
                    break;
                }

                break;
            case "PT_RAMPREP":
                if (this.Joystick.Ffb_h_Eff_Ramp(data, ref FFBPacketHandler.RampForceReport) == 0U)
                {
                    FFBPacketHandler.FFBDataUnit.RampForceReport = FFBPacketHandler.RampForceReport;
                    if (Utils.CompareObjects((object)FFBPacketHandler.OldRampForceReport,
                            (object)FFBPacketHandler.RampForceReport))
                        return;
                    FFBPacketHandler.OldRampForceReport = FFBPacketHandler.RampForceReport;
                    break;
                }

                break;
        }

        FFBPacketHandler.FFBDataUnit.PacketType = FFBPacketHandler.PacketType.ToString();
        ffbEventArgs.Data = FFBPacketHandler.FFBDataUnit;
        FFB.SendFFBData(FFBDevice);
        callback(ffbEventArgs);
    }
}