using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _TERMINAL_
{
    internal class CmdSettings : Shell.IUser
    {
        enum Codes
        {
            CursorLock,
            _last_,
        }

        IEnumerable<string> Shell.IUser.ECommands => Enumerable.Range(0, (int)Codes._last_).Select(i => ((Codes)i).ToString());

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            Shell.AddUser(new CmdSettings());
        }

        //--------------------------------------------------------------------------------------------------------------

        void Shell.IUser.OnCmdLine(in string arg0, in LineParser line)
        {
            if (Enum.TryParse(arg0, true, out Codes code) && code < Codes._last_)
                switch (code)
                {
                    case Codes.CursorLock:
                        CmdCursorLock(line);
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
            {
                if (empty)
                    //Debug.Log(Cursor.lockState);
                    ;
                else if (Enum.TryParse(arg0, true, out CursorLockMode mode))
                    Cursor.lockState = mode;
                Debug.Log(Cursor.lockState);
            }
        }
    }
}