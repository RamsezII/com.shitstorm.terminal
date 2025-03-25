using System.Text;
using UnityEngine;

namespace _TERMINAL_
{
    internal class Manual
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Commands.AddCommandKeys(OnCommand, "man", "Manual", "Help");
        }

        //----------------------------------------------------------------------------------------------------------

        static void OnCommand(in string arg0, in LineParser line)
        {
            if (line.TryRead(out _))
            {
                if (line.IsExec)
                {
                    line.cmdM |= CmdM.Man;
                    line.cmdM &= ~CmdM.Exec;
                }
                line.ReadBack();
                Terminal.instance.commands[^1].OnCmdLine(line);
            }
            else if (line.IsExec)
            {
                StringBuilder sb = new();
                for (int i = 0; i < Shell.commands._value.Length; i++)
                    sb.AppendLine($"  {i + ".",-5} {Shell.commands._value[i]}");
                sb.Log();
            }
        }
    }
}