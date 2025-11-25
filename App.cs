// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.App
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;

#nullable disable
namespace Forza_EmuWheel;

public class App : Application
{
  [DebuggerNonUserCode]
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public void InitializeComponent()
  {
    this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
  }

  [STAThread]
  [DebuggerNonUserCode]
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public static void Main()
  {
    App app = new App();
    app.InitializeComponent();
    app.Run();
  }
}
