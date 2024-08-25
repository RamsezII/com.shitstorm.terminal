using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _TERMINAL_
{
    internal class CmdSettings : IShell
    {
        enum Codes
        {
            ToggleTerminalFullScreen,
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
                    case Codes.ToggleTerminalFullScreen:
                        if (line.IsExec)
                        {
                            Terminal.instance.fullscreen = !Terminal.instance.fullscreen;
                            Debug.Log($"{typeof(Terminal).FullName}.{nameof(Terminal.instance.fullscreen)}: {Terminal.instance.fullscreen}");
                        }
                        break;

                    default:
                        Debug.LogError($"Unknown command: \"{code}\"");
                        break;
                }
        }
    }
}