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
        Joystick.Ffb_h_Type(data, ref PacketType);
        switch (PacketType)
        {
            case FFBPType.PT_CONDREP:
                if (Joystick.Ffb_h_Eff_Cond(data, ref ConditionalReport) == 0U)
                {
                    FFBDataUnit.ConditionalReport = ConditionalReport;
                    if (Utils.CompareObjects((object)OldConditionalReport,
                            (object)ConditionalReport))
                        return;
                    OldConditionalReport = ConditionalReport;
                }

                break;
            case FFBPType.PT_CONSTREP:
                if (Joystick.Ffb_h_Eff_Constant(data, ref ConstantForceReport) == 0U)
                {
                    FFBDataUnit.ConstantForceReport = ConstantForceReport;
                    if (Utils.CompareObjects((object)OldConstantForceReport,
                            (object)ConstantForceReport))
                        return;
                    OldConstantForceReport = ConstantForceReport;
                }

                break;
            case FFBPType.PT_CTRLREP:
                if (Joystick.Ffb_h_DevCtrl(data, ref Control) == 0U)
                {
                    FFBDataUnit.Control = Control;
                }

                break;
            case FFBPType.PT_EFFREP:
                if (Joystick.Ffb_h_Eff_Report(data, ref EffectReport) == 0U)
                {
                    FFBDataUnit.EffectReport = EffectReport;
                    if (Utils.CompareObjects((object)OldEffectReport,
                            (object)EffectReport))
                        return;
                    OldEffectReport = EffectReport;
                }

                break;
            case FFBPType.PT_EFOPREP:
                if (Joystick.Ffb_h_EffOp(data, ref Op) == 0U)
                {
                    OldOp = Op;
                    FFBDataUnit.Op = Op;
                }

                break;
            case FFBPType.PT_ENVREP:
                if (Joystick.Ffb_h_Eff_Envlp(data, ref EnvelopeReport) == 0U)
                {
                    FFBDataUnit.EnvelopeReport = EnvelopeReport;
                    if (Utils.CompareObjects((object)OldEnvelopeReport,
                            (object)EnvelopeReport))
                        return;
                    OldEnvelopeReport = EnvelopeReport;
                }

                break;
            case FFBPType.PT_GAINREP:
                if (Joystick.Ffb_h_DevGain(data, ref Gain) == 0U)
                {
                    FFBDataUnit.Gain = Gain;
                    if ((int)OldGain == (int)Gain)
                        return;
                    OldGain = Gain;
                }

                break;
            case FFBPType.PT_PRIDREP:
                if (Joystick.Ffb_h_Eff_Period(data, ref PeriodicReport) == 0U)
                {
                    FFBDataUnit.PeriodicReport = PeriodicReport;
                    if (Utils.CompareObjects((object)OldPeriodicReport,
                            (object)PeriodicReport))
                        return;
                    OldPeriodicReport = PeriodicReport;
                }

                break;
            case FFBPType.PT_RAMPREP:
                if (Joystick.Ffb_h_Eff_Ramp(data, ref RampForceReport) == 0U)
                {
                    FFBDataUnit.RampForceReport = RampForceReport;
                    if (Utils.CompareObjects((object)OldRampForceReport,
                            (object)RampForceReport))
                        return;
                    OldRampForceReport = RampForceReport;
                }

                break;
        }

        FFBDataUnit.PacketType = PacketType;
        ffbEventArgs.Data = FFBDataUnit;
        FFB.SendFFBData();
        callback(ffbEventArgs);
    }
}