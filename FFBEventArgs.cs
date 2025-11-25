// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.FFBEventArgs
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

using System;

#nullable disable
namespace Forza_EmuWheel;

internal class FFBEventArgs : EventArgs
{
  public FFBData Data { get; set; }
}
