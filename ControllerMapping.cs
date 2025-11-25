// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.ControllerMapping
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

#nullable disable
namespace Forza_EmuWheel;

internal class ControllerMapping
{
    public AxisData Steering { get; set; }

    public AxisData Combined { get; set; }

    public AxisData Throttle { get; set; }

    public AxisData Brake { get; set; }

    public AxisData Clutch { get; set; }

    public AxisData Handbrake { get; set; }

    public ButtonData[] Buttons { get; set; }

    public DPadData DPad { get; set; }
}