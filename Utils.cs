// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.Utils
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

using System;
using System.Reflection;

#nullable disable
namespace Forza_EmuWheel;

internal class Utils
{
  public static bool CompareObjects(object oldObj, object newObj)
  {
    FieldInfo[] fields1 = oldObj.GetType().GetFields();
    FieldInfo[] fields2 = newObj.GetType().GetFields();
    for (int index = 0; index < fields1.Length; ++index)
    {
      fields1[index].GetValue(oldObj);
      fields2[index].GetValue(newObj);
      if (!fields1[index].GetValue(oldObj).Equals(fields2[index].GetValue(newObj)))
        return false;
    }
    return true;
  }

  public static int Polar2Degrees(byte polar) => (int) polar * 360 / (int) byte.MaxValue;

  public static double Degrees2Radian(double angle) => Math.PI * angle / 180.0;

  public static int CombinedAxisSplit(int value, AxisData mapping, int inverted)
  {
    if (value > (int) short.MaxValue + (int) ((double) mapping.Deadzone * (double) ushort.MaxValue / 2.0))
    {
      value = (int) Math.Round((((double) value - (double) mapping.Deadzone * (double) ushort.MaxValue) / (1.0 - (double) mapping.Deadzone) - (double) short.MaxValue) * 2.0);
      if (inverted == 0)
        value = Math.Abs(value - (int) ushort.MaxValue);
    }
    else if (value < (int) short.MaxValue - (int) ((double) mapping.Deadzone * (double) ushort.MaxValue / 2.0))
    {
      value = Math.Abs(value - (int) ushort.MaxValue);
      value = (int) Math.Round((((double) value - (double) mapping.Deadzone * (double) ushort.MaxValue) / (1.0 - (double) mapping.Deadzone) - (double) short.MaxValue) * 2.0);
      if (inverted == 0)
        value = Math.Abs(value - (int) ushort.MaxValue);
    }
    else
      value = inverted != 1 ? (int) ushort.MaxValue : 0;
    return value;
  }

  public static int DeadzoneAndInvert(int value, AxisData mapping, bool steering)
  {
    if (mapping.Inverted != 0)
      value = Math.Abs(value - (int) ushort.MaxValue);
    if (steering)
    {
      if (value < (int) short.MaxValue && Math.Abs(value - (int) short.MaxValue) < (int) ((double) mapping.Deadzone * 32768.0))
        value = (int) short.MaxValue;
      else if (value > (int) short.MaxValue && value - (int) short.MaxValue < (int) ((double) mapping.Deadzone * 32768.0))
        value = (int) short.MaxValue;
      else if (value < (int) short.MaxValue - (int) ((double) mapping.Deadzone * (double) ushort.MaxValue / 2.0))
      {
        value = Math.Abs(value - (int) ushort.MaxValue);
        value = (int) Math.Round((double) (value - (int) ((double) mapping.Deadzone * (double) ushort.MaxValue)) / (double) ((int) ushort.MaxValue - (int) ((double) mapping.Deadzone * (double) ushort.MaxValue)) * (double) ushort.MaxValue, 0);
        value = Math.Abs(value - (int) ushort.MaxValue);
      }
      else if (value > (int) short.MaxValue + (int) ((double) mapping.Deadzone * (double) ushort.MaxValue / 2.0))
        value = (int) Math.Round((double) (value - (int) ((double) mapping.Deadzone * (double) ushort.MaxValue)) / (double) ((int) ushort.MaxValue - (int) ((double) mapping.Deadzone * (double) ushort.MaxValue)) * (double) ushort.MaxValue, 0);
    }
    else if (mapping.Inverted == 0 && value > Math.Abs((int) ((double) mapping.Deadzone * (double) ushort.MaxValue)) && (double) mapping.Deadzone != 0.0)
      value = (int) ushort.MaxValue;
    else if (mapping.Inverted == 1 && value < (int) ((double) mapping.Deadzone * (double) ushort.MaxValue) && (double) mapping.Deadzone != 0.0)
      value = 0;
    else if (mapping.Inverted == 0 || mapping.Inverted == -1)
    {
      value = Math.Abs(value - (int) ushort.MaxValue);
      value = (int) Math.Round((double) (value - (int) ((double) mapping.Deadzone * (double) ushort.MaxValue)) / (double) ((int) ushort.MaxValue - (int) ((double) mapping.Deadzone * (double) ushort.MaxValue)) * (double) ushort.MaxValue, 0);
      value = Math.Abs(value - (int) ushort.MaxValue);
    }
    else
      value = (int) Math.Round((double) (value - (int) ((double) mapping.Deadzone * (double) ushort.MaxValue)) / (double) ((int) ushort.MaxValue - (int) ((double) mapping.Deadzone * (double) ushort.MaxValue)) * (double) ushort.MaxValue, 0);
    return value;
  }

  public static byte[] PackBoolsInByteArray(bool[] bools)
  {
    int length1 = bools.Length;
    int length2 = length1 >> 3;
    if ((length1 & 7) != 0)
      ++length2;
    byte[] numArray = new byte[length2];
    for (int index = 0; index < bools.Length; ++index)
    {
      if (bools[index])
        numArray[index >> 3] |= (byte) (1 << (index & 7));
    }
    return numArray;
  }
}
