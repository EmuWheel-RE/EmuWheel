// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.InputCollector
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Forza_EmuWheel;

internal class InputCollector
{
  public static List<Controller> Controllers { get; set; }

  public static List<Joystick> GameControllers { get; set; }

  public int GetButtonCount()
  {
    int buttonCount = 0;
    foreach (Controller controller in InputCollector.Controllers)
      buttonCount += controller.Buttons.Count<Button>();
    return buttonCount;
  }

  public DPadData GetDPadData()
  {
    DPadData dpadData = new DPadData();
    foreach (Controller controller1 in InputCollector.Controllers)
    {
      Controller controller = controller1;
      if (controller.DPad != null)
      {
        DPad dpad = (DPad) null;
        if (controller.DPad != null)
          dpad = controller.DPad;
        if (dpad != null)
        {
          dpadData.ControllerIndex = InputCollector.GameControllers.FindIndex((Predicate<Joystick>) (x => x.Information.InstanceGuid == controller.InstanceGuid));
          dpadData.DPadIndex = dpad.Index;
          return dpadData;
        }
      }
    }
    return (DPadData) null;
  }

  public List<ButtonData> GetButtonData(int index)
  {
    List<ButtonData> buttonData1 = new List<ButtonData>();
    foreach (Controller controller1 in InputCollector.Controllers)
    {
      Controller controller = controller1;
      if (controller.Buttons != null)
      {
        List<Button> buttonList = new List<Button>();
        if (controller.Buttons.Where<Button>((Func<Button, bool>) (x => x.Id == (Button.ButtonEnum) index)).Select<Button, Button>((Func<Button, Button>) (x => x)).Any<Button>())
          buttonList = controller.Buttons.Where<Button>((Func<Button, bool>) (x => x.Id == (Button.ButtonEnum) index)).Select<Button, Button>((Func<Button, Button>) (x => x)).ToList<Button>();
        foreach (Button button in buttonList)
        {
          ButtonData buttonData2 = new ButtonData()
          {
            ButtonIndex = button.Index,
            ButtonBind = (int) button.Id,
            ControllerIndex = InputCollector.GameControllers.FindIndex((Predicate<Joystick>) (x => x.Information.InstanceGuid == controller.InstanceGuid))
          };
          buttonData1.Add(buttonData2);
        }
      }
    }
    return buttonData1;
  }

  public AxisData GetAxisData(string axis)
  {
    AxisData axisData = new AxisData();
    foreach (Controller controller1 in InputCollector.Controllers)
    {
      Controller controller = controller1;
      if (controller.Axes != null)
      {
        Axis axis1 = controller.Axes.Where<Axis>((Func<Axis, bool>) (x => x.Id == axis)).Select<Axis, Axis>((Func<Axis, Axis>) (x => x)).FirstOrDefault<Axis>();
        if (axis1 != null)
        {
          axisData.AxisIndex = axis1.AxisIndex;
          axisData.Deadzone = axis1.Deadzone;
          axisData.Inverted = axis1.Inverted;
          axisData.ControllerIndex = InputCollector.GameControllers.FindIndex((Predicate<Joystick>) (x => x.Information.InstanceGuid == controller.InstanceGuid));
          return axisData;
        }
      }
    }
    return (AxisData) null;
  }
}
