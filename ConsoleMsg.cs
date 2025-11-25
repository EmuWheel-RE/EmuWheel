// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.ConsoleMsg
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

using System.ComponentModel;

#nullable disable
namespace Forza_EmuWheel;

internal class ConsoleMsg : INotifyPropertyChanged
{
  private string message;
  public static ConsoleMsg Msg = new ConsoleMsg();
  private bool stopIsEnabled;
  private bool startIsEnabled;

  public event PropertyChangedEventHandler PropertyChanged;

  public bool StopIsEnabled
  {
    get => this.stopIsEnabled;
    set
    {
      this.stopIsEnabled = value;
      this.OnPropertyChanged(nameof (StopIsEnabled));
    }
  }

  public bool StartIsEnabled
  {
    get => this.startIsEnabled;
    set
    {
      this.startIsEnabled = value;
      this.OnPropertyChanged(nameof (StartIsEnabled));
    }
  }

  public string Message
  {
    get => this.message;
    set
    {
      this.message = value;
      this.OnPropertyChanged(nameof (Message));
    }
  }

  protected void OnPropertyChanged(string value)
  {
    PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
    if (propertyChanged == null)
      return;
    propertyChanged((object) this, new PropertyChangedEventArgs(value));
  }
}
