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
            CursorLock,
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
                    case Codes.CursorLock:
                        CmdCursorLock(line);
                        break;

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

        static void CmdCursorLock(in LineParser line)
        {
            bool empty = !line.TryRead(out string arg0);
            if (line.IsCplThis)
                line.OnCpls(arg0, Enumerable.Range(0, 3).Select(i => ((CursorLockMode)i).ToString()));
            else if (line.IsExec)
                if (empty)
                    Debug.Log(Cursor.lockState);
                else if (Enum.TryParse(arg0, true, out CursorLockMode mode))
                {
                    Cursor.lockState = mode;
                    Debug.Log($"{nameof(Cursor)}.{nameof(Cursor.lockState)}: \"{Cursor.lockState}\"");
                }

        }
    }
}