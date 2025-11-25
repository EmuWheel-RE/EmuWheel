// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.FFBData
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

using vJoyInterfaceWrap;

#nullable disable
namespace Forza_EmuWheel;

internal class FFBData
{
    public vJoy.FFB_EFF_REPORT EffectReport { get; set; }

    public vJoy.FFB_EFF_ENVLP EnvelopeReport { get; set; }

    public vJoy.FFB_EFF_COND ConditionalReport { get; set; }

    public vJoy.FFB_EFF_PERIOD PeriodicReport { get; set; }

    public vJoy.FFB_EFF_CONSTANT ConstantForceReport { get; set; }

    public vJoy.FFB_EFF_RAMP RampForceReport { get; set; }

    public vJoy.FFB_EFF_OP Op { get; set; }

    public FFB_CTRL Control { get; set; }

    public byte Gain { get; set; }

    public string PacketType { get; set; }
}