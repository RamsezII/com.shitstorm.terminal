using _TERMINAL_;
using _UTIL_;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _ATHENA_
{
    internal class AthenaTCP : IShell
    {
        public enum Commands : byte
        {
            Athena,
            _last_,
        }

        IEnumerable<string> IShell.ECommands => Enum.GetNames(typeof(Commands));

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            Shell.AddUser(new AthenaTCP());
        }

        //--------------------------------------------------------------------------------------------------------------

        void IShell.OnCmdLine(in string arg0, in LineParser line)
        {
            if (Enum.TryParse(arg0, true, out Commands code) && code < Commands._last_)
                switch (code)
                {
                    case Commands.Athena:
                        Terminal.instance.commands.Add(new CmdAthena(line));
                        break;
                    default:
                        Debug.LogWarning($"Unimplemented command: {arg0}");
                        break;
                }
            else
                Debug.LogWarning($"Unknown command: {arg0}");
        }

        internal class CmdAthena : Command
        {
            readonly string input;
            readonly StringBuilder history = new();
            readonly ThreadSafe<bool> blocked = new();

            //--------------------------------------------------------------------------------------------------------------

            public CmdAthena(in LineParser line)
            {
                input = line.ReadAll();
                ToggleLock(true);
                Util.SetFlags(ref flags, Flags.Status, false);
                ToggleLock(false);
                if (!string.IsNullOrWhiteSpace(input))
                    OnCmdLine(new LineParser(input, CmdF.exec));
            }

            //--------------------------------------------------------------------------------------------------------------

            public void ToggleLock(in bool value)
            {
                lock (blocked)
                {
                    blocked._value = value;
                    Util.SetFlags(ref flags, Flags.Killable | Flags.Stdin, !value);
                }
            }

            //--------------------------------------------------------------------------------------------------------------

            public override void OnCmdLine(in LineParser line)
            {
                string input = line.ReadAll();

                lock (blocked)
                {
                    if (blocked._value)
                        return;
                    ToggleLock(true);
                    Util.SetFlags(ref flags, Flags.Status, true);
                }

                Task task = Task.Run(() => Util_net.TryTCP(Util_net.DOMAIN_3VE, Util_net.PORT_ATHENA, false, Dispose,
                (tcp, writer, reader) =>
                {
                    lock (history)
                        history.AppendLine($"hero:\n{input}");

                    writer.WriteText(history.ToString());
                    string response = reader.ReadText();

                    Debug.Log($"{typeof(AthenaTCP).FullName}: {response}");

                    history.AppendLine($"\nATHENA: {response}\n");

                    lock (blocked)
                    {
                        ToggleLock(false);
                        Util.SetFlags(ref flags, Flags.Status, false);
                    }
                }));
            }
        }
    }
}