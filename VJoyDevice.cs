// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.VJoyDevice
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

using System;
using System.Collections.Generic;
using vJoyInterfaceWrap;

#nullable disable
namespace Forza_EmuWheel;

internal class VJoyDevice
{
  public static vJoy Joystick;
  public static vJoy.JoystickState IReport;
  public static uint ID = 1;

  public static bool InitializeVJoy()
  {
    VJoyDevice.Joystick = new vJoy();
    VJoyDevice.IReport = new vJoy.JoystickState();
    if (!VJoyDevice.Joystick.vJoyEnabled())
    {
      ConsoleMsg msg = ConsoleMsg.Msg;
      msg.Message = $"{msg.Message}[ERROR] vJoy driver not enabled: Failed Getting vJoy attributes.{Environment.NewLine}";
      return false;
    }
    VjdStat vjdStatus = VJoyDevice.Joystick.GetVJDStatus(VJoyDevice.ID);
    switch (vjdStatus)
    {
      case VjdStat.VJD_STAT_OWN:
        ConsoleMsg msg1 = ConsoleMsg.Msg;
        msg1.Message = $"{msg1.Message}[ERROR] vJoy Device {(object) VJoyDevice.ID} is already owned by this feeder.{Environment.NewLine}";
        return false;
      case VjdStat.VJD_STAT_FREE:
        Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
        bool vjdAxisExist1 = VJoyDevice.Joystick.GetVJDAxisExist(VJoyDevice.ID, HID_USAGES.HID_USAGE_X);
        bool vjdAxisExist2 = VJoyDevice.Joystick.GetVJDAxisExist(VJoyDevice.ID, HID_USAGES.HID_USAGE_Y);
        bool vjdAxisExist3 = VJoyDevice.Joystick.GetVJDAxisExist(VJoyDevice.ID, HID_USAGES.HID_USAGE_Z);
        bool vjdAxisExist4 = VJoyDevice.Joystick.GetVJDAxisExist(VJoyDevice.ID, HID_USAGES.HID_USAGE_RX);
        bool vjdAxisExist5 = VJoyDevice.Joystick.GetVJDAxisExist(VJoyDevice.ID, HID_USAGES.HID_USAGE_RY);
        bool vjdAxisExist6 = VJoyDevice.Joystick.GetVJDAxisExist(VJoyDevice.ID, HID_USAGES.HID_USAGE_RZ);
        bool vjdAxisExist7 = VJoyDevice.Joystick.GetVJDAxisExist(VJoyDevice.ID, HID_USAGES.HID_USAGE_SL0);
        bool vjdAxisExist8 = VJoyDevice.Joystick.GetVJDAxisExist(VJoyDevice.ID, HID_USAGES.HID_USAGE_SL1);
        dictionary.Add("X", vjdAxisExist1);
        dictionary.Add("Y", vjdAxisExist2);
        dictionary.Add("Z", vjdAxisExist3);
        dictionary.Add("RX", vjdAxisExist4);
        dictionary.Add("RY", vjdAxisExist5);
        dictionary.Add("RZ", vjdAxisExist6);
        dictionary.Add("Slider", vjdAxisExist7);
        dictionary.Add("Dial/Slider1", vjdAxisExist8);
        int vjdButtonNumber = VJoyDevice.Joystick.GetVJDButtonNumber(VJoyDevice.ID);
        int vjdContPovNumber = VJoyDevice.Joystick.GetVJDContPovNumber(VJoyDevice.ID);
        int vjdDiscPovNumber = VJoyDevice.Joystick.GetVJDDiscPovNumber(VJoyDevice.ID);
        foreach (KeyValuePair<string, bool> keyValuePair in dictionary)
        {
          if (!keyValuePair.Value)
          {
            ConsoleMsg msg2 = ConsoleMsg.Msg;
            msg2.Message = $"{msg2.Message}[ERROR] vJoy Device {(object) VJoyDevice.ID} is missing axis: {keyValuePair.Key}{Environment.NewLine}";
            return false;
          }
        }
        if (vjdButtonNumber != 128 /*0x80*/)
        {
          ConsoleMsg msg3 = ConsoleMsg.Msg;
          msg3.Message = $"{msg3.Message}[ERROR] vJoy Device {(object) VJoyDevice.ID}  has incorrect number of buttons: Expected: {(object) 128 /*0x80*/} Reported: {(object) vjdButtonNumber}{Environment.NewLine}";
          return false;
        }
        if (vjdDiscPovNumber != 1)
        {
          ConsoleMsg msg4 = ConsoleMsg.Msg;
          msg4.Message = $"{msg4.Message}[ERROR] vJoy Device {(object) VJoyDevice.ID}  has incorrect number of 4-way hat switches (Dpads): Expected: {(object) 1} Reported: {(object) vjdDiscPovNumber}{Environment.NewLine}";
          return false;
        }
        if (vjdContPovNumber > 0)
        {
          ConsoleMsg msg5 = ConsoleMsg.Msg;
          msg5.Message = $"{msg5.Message}[ERROR] vJoy Device {(object) VJoyDevice.ID}  has incorrect hat switch type: Expected: 4-way Reported: Continuous{Environment.NewLine}";
          return false;
        }
        uint DllVer = 0;
        uint DrvVer = 0;
        if (!VJoyDevice.Joystick.DriverMatch(ref DllVer, ref DrvVer))
        {
          ConsoleMsg.Msg.Message += "[ERROR] vJoy driver version does not match the version of 'vJoyInterface.dll'";
          return false;
        }
        switch (vjdStatus)
        {
          case VjdStat.VJD_STAT_OWN:
            ConsoleMsg msg6 = ConsoleMsg.Msg;
            msg6.Message = $"{msg6.Message}[ERROR] Failed to acquire vJoy device number {(object) VJoyDevice.ID}";
            return false;
          case VjdStat.VJD_STAT_FREE:
            if (VJoyDevice.Joystick.AcquireVJD(VJoyDevice.ID))
              break;
            goto case VjdStat.VJD_STAT_OWN;
        }
        ConsoleMsg msg7 = ConsoleMsg.Msg;
        msg7.Message = $"{msg7.Message}[SUCCESS] vJoy device number {(object) VJoyDevice.ID} acquired!{Environment.NewLine}";
        return true;
      case VjdStat.VJD_STAT_BUSY:
        ConsoleMsg msg8 = ConsoleMsg.Msg;
        msg8.Message = $"{msg8.Message}[ERROR] vJoy Device {(object) VJoyDevice.ID} is already owned by another feeder. Cannot continue.{Environment.NewLine}";
        return false;
      case VjdStat.VJD_STAT_MISS:
        ConsoleMsg msg9 = ConsoleMsg.Msg;
        msg9.Message = $"{msg9.Message}[ERROR] vJoy Device {(object) VJoyDevice.ID} is disabled. Cannot continue.{Environment.NewLine}";
        return false;
      default:
        ConsoleMsg msg10 = ConsoleMsg.Msg;
        msg10.Message = $"{msg10.Message}[ERROR] vJoy Device {(object) VJoyDevice.ID} general error. Cannot continue.{Environment.NewLine}";
        return false;
    }
  }
}
