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
            if (line.IsExec)
            {
                line.cmdM |= CmdM.Man;
                line.cmdM &= ~CmdM.Exec;
            }
            Terminal.instance.commands[^1].OnCmdLine(line);
        }
    }
}