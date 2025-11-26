// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.MainWindow
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;

namespace Forza_EmuWheel;

public partial class MainWindow : Window, IComponentConnector
{
  private const string VJoyProductGuid = "bead1234000000000000504944564944";

  private static SharpDX.DirectInput.DirectInput DI { get; set; }

  private IntPtr MainWindowHandle { get; set; }

  private FeederTask? FeederTask;
  private FFB? _ffb;

  public MainWindow()
  {
    this.InitializeComponent();
    this.DataContext = (object) ConsoleMsg.Msg;
    this.Loaded += new RoutedEventHandler(this.OnLoaded);
  }

  private void OnLoaded(object sender, RoutedEventArgs e)
  {
    try
    {
      MainWindow.DI = new SharpDX.DirectInput.DirectInput();
      this.MainWindowHandle = new WindowInteropHelper((Window) this).Handle;
      if (MainWindow.DI == null)
      {
        ConsoleMsg msg = ConsoleMsg.Msg;
        msg.Message = $"{msg.Message}[ERROR] Could not initialize DirectInput.{Environment.NewLine}";
      }
      else if (!VJoyDevice.InitializeVJoy())
      {
        ConsoleMsg msg = ConsoleMsg.Msg;
        msg.Message = $"{msg.Message}[ERROR] Could not initialize vJoy.{Environment.NewLine}";
      }
      else if (!this.GetGameControllers())
      {
        ConsoleMsg msg = ConsoleMsg.Msg;
        msg.Message = $"{msg.Message}[ERROR] Could not load configuration.{Environment.NewLine}";
      }
      else if (InputCollector.GameControllers == null)
      {
        ConsoleMsg msg = ConsoleMsg.Msg;
        msg.Message = $"{msg.Message}[ERROR] No controllers connected.{Environment.NewLine}";
      }
      else
      {
        if (!this.AllDevicesPresent())
          return;
        this.InitializeFFB();
        ConsoleMsg msg = ConsoleMsg.Msg;
        msg.Message = $"{msg.Message}[INFO] Forza EmuWheel is ready to start...{Environment.NewLine}";
        ConsoleMsg.Msg.StartIsEnabled = true;

        if (Environment.GetCommandLineArgs().Contains("--start"))
        {
          StartFeeder();
        }
      }
    }
    catch (Exception ex)
    {
      int num = (int) MessageBox.Show(ex.ToString());
    }
  }

  private bool InitializeFFB()
  {
    Controller ffbDev = InputCollector.Controllers.Where<Controller>((Func<Controller, bool>) (x => x.FFBParameters != null)).Select<Controller, Controller>((Func<Controller, Controller>) (x => x)).FirstOrDefault<Controller>();
    if (ffbDev == null)
    {
      ConsoleMsg msg = ConsoleMsg.Msg;
      msg.Message = $"{msg.Message}[INFO] No force feedback device defined in configuration. Force feedback is disabled.{Environment.NewLine}";
      return false;
    }
    ConsoleMsg msg1 = ConsoleMsg.Msg;
    msg1.Message = $"{msg1.Message}[INFO] Force Feedback device: '{ffbDev.InstanceName}'.{Environment.NewLine}";
    Joystick joystick = InputCollector.GameControllers.Where<Joystick>((Func<Joystick, bool>) (x => x.Information.InstanceGuid == ffbDev.InstanceGuid)).FirstOrDefault<Joystick>();
    if (joystick == null)
      return false;
    this.GetActuators((Device) joystick);
    _ffb = new FFB(joystick, VJoyDevice.Joystick);
    return true;
  }

  private bool AllDevicesPresent()
  {
    bool flag = true;
    foreach (Controller controller in InputCollector.Controllers)
    {
      Controller contr = controller;
      if (!InputCollector.GameControllers.Where<Joystick>((Func<Joystick, bool>) (x => x.Information.InstanceGuid == contr.InstanceGuid)).Any<Joystick>())
      {
        ConsoleMsg msg = ConsoleMsg.Msg;
        msg.Message = $"{msg.Message}[ERROR] Device '{contr.InstanceName}' is not connected.{Environment.NewLine}";
        flag = false;
      }
    }
    return flag;
  }

  private bool GetGameControllers()
  {
    Guid guid = Guid.Parse("bead1234000000000000504944564944");
    InputCollector.Controllers = Controller.GetConfigurationData();
    if (InputCollector.Controllers == null)
    {
      ConsoleMsg msg = ConsoleMsg.Msg;
      msg.Message = $"{msg.Message}[ERROR] No controllers defined in 'configuration.json'.{Environment.NewLine}";
      return false;
    }
    InputCollector.GameControllers = new List<Joystick>();
    foreach (DeviceInstance device1 in (IEnumerable<DeviceInstance>) MainWindow.DI.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly))
    {
      DeviceInstance device = device1;
      if (!(device.ProductGuid == guid))
      {
        try
        {
          if (InputCollector.Controllers.Where<Controller>((Func<Controller, bool>) (x => x.InstanceGuid == device.InstanceGuid)).Any<Controller>())
          {
            Joystick joystick = new Joystick(MainWindow.DI, device.InstanceGuid);
            if (InputCollector.Controllers.Where<Controller>((Func<Controller, bool>) (x => x.InstanceGuid == device.InstanceGuid && x.FFBParameters != null)).Any<Controller>())
              joystick.SetCooperativeLevel(this.MainWindowHandle, CooperativeLevel.Exclusive | CooperativeLevel.Background);
            InputCollector.GameControllers.Add(joystick);
            ConsoleMsg msg = ConsoleMsg.Msg;
            msg.Message = $"{msg.Message}[INFO] Found game controller '{joystick.Information.InstanceName.ToString()}'.{Environment.NewLine}";
          }
        }
        catch
        {
          ConsoleMsg msg = ConsoleMsg.Msg;
          msg.Message = $"{msg.Message}[ERROR] Could not acquire device '{InputCollector.Controllers.Where<Controller>((Func<Controller, bool>) (x => x.InstanceGuid == device.InstanceGuid)).Select<Controller, string>((Func<Controller, string>) (x => x.InstanceName)).FirstOrDefault<string>()}'.{Environment.NewLine}";
          return false;
        }
      }
    }
    if (InputCollector.GameControllers.Count > 0)
    {
      ConsoleMsg msg = ConsoleMsg.Msg;
      msg.Message = $"{msg.Message}[SUCCESS] Communication with controller(s) established.{Environment.NewLine}";
    }
    return true;
  }

  private void GetActuators(Device dev)
  {
    FFB.ActuatorsObjectTypes = new List<int>();
    foreach (DeviceObjectInstance deviceObjectInstance in (IEnumerable<DeviceObjectInstance>) dev.GetObjects())
    {
      if ((deviceObjectInstance.ObjectId.Flags & DeviceObjectTypeFlags.ForceFeedbackActuator) != DeviceObjectTypeFlags.All)
        FFB.ActuatorsObjectTypes.Add(Convert.ToInt32((object) deviceObjectInstance.ObjectId.Flags));
    }
  }

  private void Console_TextChanged(object sender, TextChangedEventArgs e)
  {
    this.console.Focus();
    this.console.CaretIndex = this.console.Text.Length;
    this.console.ScrollToEnd();
  }

  private void Start_Click(object sender, RoutedEventArgs e)
  {
    StartFeeder();
  }
  
  private void StartFeeder() {
    ConsoleMsg msg = ConsoleMsg.Msg;
    msg.Message = $"{msg.Message}[INFO] Forza EmuWheel is running...{Environment.NewLine}";
    ConsoleMsg.Msg.StartIsEnabled = false;
    ConsoleMsg.Msg.StopIsEnabled = true;
    this.FeederTask = new FeederTask();
  }

  private void Stop_Click(object sender, RoutedEventArgs e)
  {
    ConsoleMsg msg = ConsoleMsg.Msg;
    msg.Message = $"{msg.Message}[INFO] Forza EmuWheel was stopped.{Environment.NewLine}";
    ConsoleMsg.Msg.StartIsEnabled = true;
    ConsoleMsg.Msg.StopIsEnabled = false;
    this.FeederTask?.CancelAndWait();
    this.FeederTask = null;
  }

  private void OnApplicationExit(object sender, EventArgs e)
  {
    this.FeederTask?.CancelAndWait();
  }
}
