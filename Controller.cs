// Decompiled with JetBrains decompiler
// Type: Forza_EmuWheel.Controller
// Assembly: Forza EmuWheel, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F67B691-4378-49D8-9752-4B9D9B08BEF0
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Forza EmuWheel.exe

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Forza_EmuWheel;

public class Controller
{
    public Guid ProductGuid { get; set; }

    public Guid InstanceGuid { get; set; }

    public string InstanceName { get; set; }

    public string ProductName { get; set; }

    public List<Axis> Axes { get; set; }

    public List<Button> Buttons { get; set; }

    public DPad DPad { get; set; }

    public FFBParams FFBParameters { get; set; }

    public static List<Controller> GetConfigurationData()
    {
        if (!File.Exists("configuration.json"))
            return (List<Controller>)null;
        try
        {
            return JsonConvert.DeserializeObject<List<Controller>>(File.ReadAllText("configuration.json"));
        }
        catch (Exception ex)
        {
            ConsoleMsg msg = ConsoleMsg.Msg;
            msg.Message = $"{msg.Message}[ERROR] {ex.Message}{Environment.NewLine}";
            return (List<Controller>)null;
        }
    }
}