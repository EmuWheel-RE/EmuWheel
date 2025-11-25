// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.Axis
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

#nullable disable
namespace Forza_EmuWheel;

public class Axis
{
  public string Id { get; set; }

  public int AxisIndex { get; set; }

  public int Inverted { get; set; }

  public float Deadzone { get; set; }

  public enum AxisEnum
  {
    X,
    Y,
    Z,
    RotationX,
    RotationY,
    RotationZ,
    Sliders0,
    Sliders1,
  }
}
