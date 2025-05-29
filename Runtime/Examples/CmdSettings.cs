using _ARK_;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace _TERMINAL_
{
    internal class CmdSettings : IShell
    {
        enum Codes
        {
            Edit,
            OpenTerminal,
            CloseTerminal,
            ToggleTerminalFullScreen,
            machine_name,
            Shutdown,
            _last_,
        }

        IEnumerable<string> IShell.ECommands => Enumerable.Range(0, (int)Codes._last_).Select(i => ((Codes)i).ToString());

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            Shell.AddUser(new CmdSettings());
        }

        //--------------------------------------------------------------------------------------------------------------

        void IShell.OnCmdLine(in string arg0, in LineParser line)
        {
            if (Enum.TryParse(arg0, true, out Codes code) && code < Codes._last_)
                switch (code)
                {
                    case Codes.Edit:
                        OnCmdEdit(line);
                        break;

                    case Codes.OpenTerminal:
                        if (line.IsExec)
                            Terminal.instance.ToggleWindow(true);
                        break;

                    case Codes.CloseTerminal:
                        if (line.IsExec)
                            Terminal.instance.ToggleWindow(false);
                        break;

                    case Codes.ToggleTerminalFullScreen:
                        if (line.IsExec)
                        {
                            Terminal.instance.fullscreen = !Terminal.instance.fullscreen;
                            Debug.Log($"{typeof(Terminal).FullName}.{nameof(Terminal.instance.fullscreen)}: {Terminal.instance.fullscreen}");
                        }
                        break;

                    case Codes.machine_name:
                        {
                            string netName = line.Read();
                            if (line.IsExec)
                            {
                                if (string.IsNullOrWhiteSpace(netName))
                                    Debug.Log(ArkMachine.user_name.Value);
                                else
                                    ArkMachine.user_name.Update(netName);
                            }
                        }
                        break;

                    case Codes.Shutdown:
                        if (line.IsExec)
                            Application.Quit();
                        break;

                    default:
                        Debug.LogError($"Unknown command: \"{code}\"");
                        break;
                }
        }


        void OnCmdEdit(in LineParser line)
        {
            string path = line.ReadAsPath();
            if (line.IsExec)
                try
                {
                    FileInfo file = new(path);
                    if (!File.Exists(file.FullName))
                    {
                        string folderPath = Path.GetDirectoryName(file.FullName);
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);
                        Debug.Log($"Creating file: \"{file.FullName}\"".ToSubLog());
                        File.WriteAllText(file.FullName, string.Empty);
                    }
                    Debug.Log($"Opening file: \"{file.FullName}\"".ToSubLog());
                    Application.OpenURL(file.FullName);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
        }
    }
}