using System.Text;
using UnityEngine;

namespace _TERMINAL_
{
    internal class Manual
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Shell.AddCommand(new(null, "Manual", onCmd_line: line =>
            {
                line.cmdM |= CmdM.Man;
                if (line.TryRead(out _))
                {
                    line.ReadBack();
                    if (line.IsExec)
                        line.cmdM &= ~CmdM.Exec;
                    Terminal.instance.commands[^1].OnCmdLine(line);
                }
                else if (line.IsExec)
                {
                    StringBuilder sb = new();
                    foreach (var pair in Shell._commands)
                        sb.AppendLine($"  {pair.Key}");
                    sb.Log();
                }
            }),
            "man", "Help");
        }
    }
}