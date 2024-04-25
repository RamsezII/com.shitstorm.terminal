using System.Collections.Generic;
using UnityEngine;

namespace _TERMINAL_
{
    public abstract partial class Shell
    {
        public readonly Terminal terminal;
        public readonly List<Process> processes = new();

        public const string
            UserColor = "#73CC26",
            BoaColor = "#73B2D9";

        public string userName, prefixe;

        //----------------------------------------------------------------------------------------------------------

        public Shell(in Terminal terminal)
        {
            this.terminal = terminal;
            terminal.shell = this;
            prefixe = Prefixe(userName);
        }

        //----------------------------------------------------------------------------------------------------------

        public static string Prefixe(in string user, in string cmd = "~") => $"{user.SetColor(UserColor)}:{cmd.SetColor(BoaColor)}$ ";

        public void SigKill()
        {
            Process process = processes[^1];

            if (process.flags.HasFlag(Process.Flags.Stdin))
                Debug.Log(prefixe);

            Debug.Log("^C");

            if (process.flags.HasFlag(Process.Flags.Killable))
            {
                process.Kill();
                Event.current.Use();
            }
            else
                Debug.LogWarning($"can not abort \"{process.GetType()}\"");
        }

        public void CmdLine(in LineParser line) => OnCmdLine(line.Read(), line);
        public virtual void OnCmdLine(in string arg0, in LineParser line) => Debug.LogWarning($"{GetType()} does not implement \"{arg0.Bold()}\"");
    }
}