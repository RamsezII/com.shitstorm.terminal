using System;
using System.Collections.Generic;
using UnityEngine;

namespace _TERMINAL_
{
    public sealed partial class Shell : Command
    {
        public interface ICommand
        {
            void OnCmdLine(LineParser line);
        }

        public readonly struct Namespace : ICommand
        {
            public readonly object owner;
            public readonly Dictionary<string, ICommand> _commands;

            //----------------------------------------------------------------------------------------------------------

            public Namespace(in object owner, in StringComparer comparer = null)
            {
                this.owner = owner;
                _commands = new(comparer ?? StringComparer.OrdinalIgnoreCase);
            }

            //----------------------------------------------------------------------------------------------------------

            public void AddCommand(in CommandInfos infos, params string[] aliases)
            {
                _commands.Add(infos.key, infos);
                for (int i = 0; i < aliases.Length; i++)
                    _commands.Add(aliases[i], infos);
            }

            public void RemoveByOwner(in object owner)
            {
                if (owner == null)
                    throw new ArgumentNullException(nameof(owner));

                Dictionary<string, ICommand> copy = new(_commands);
                _commands.Clear();

                foreach (var pair in copy)
                    switch (pair.Value)
                    {
                        case Namespace n:
                            if (owner != n.owner)
                            {
                                n.RemoveByOwner(owner);
                                _commands.Add(pair.Key, n);
                            }
                            break;

                        case CommandInfos c:
                            if (owner != c.owner)
                                _commands.Add(pair.Key, c);
                            break;
                    }
            }

            public void OnCmdLine(LineParser line)
            {
                string arg0 = line.Read();
                if (line.IsCplThis)
                    line.OnCpls(arg0, _commands.Keys);
                else if (_commands.TryGetValue(arg0, out ICommand icmd))
                {
                    icmd.OnCmdLine(line);
                    if (line.IsExec)
                        line.cmdM |= CmdM._history;
                }
                else if (line.IsExec)
                    Debug.LogWarning($"no command named \"{arg0}\"");
            }
        }

        public readonly struct CommandInfos : ICommand
        {
            public readonly object owner;
            public readonly string key;
            internal readonly Action onCmd_exe;
            internal readonly Action<LineParser> onCmd_line;
            internal readonly Action<bool> onCmd_bool;

            //----------------------------------------------------------------------------------------------------------

            public CommandInfos(in object owner, in string key,
                in Action onCmd_exe = null,
                in Action<bool> onCmd_bool = null,
                in Action<LineParser> onCmd_line = null)
            {
                this.owner = owner;
                this.key = key;
                this.onCmd_exe = onCmd_exe;
                this.onCmd_bool = onCmd_bool;
                this.onCmd_line = onCmd_line;
            }

            void ICommand.OnCmdLine(LineParser line)
            {
                if (line.IsExec)
                    onCmd_exe?.Invoke();

                onCmd_line?.Invoke(line);

                if (onCmd_bool != null)
                {
                    bool notEmpty = line.TryRead(out string arg1);
                    if (line.IsCplThis)
                        line.OnCpls(arg1, "TRUE", "FALSE");
                    else if (line.IsExec)
                        if (notEmpty)
                            if (bool.TryParse(arg1, out bool toggle))
                                onCmd_bool(toggle);
                            else
                                Debug.LogWarning($"{this} in command \"{arg1}\": could not parse boolean \"{arg1}\"");
                        else
                            Debug.LogWarning($"{this} in command \"{arg1}\": expected boolean");
                }
            }
        }

        public static readonly Namespace root_commands = new(owner: null, comparer: StringComparer.OrdinalIgnoreCase);

        public static readonly Shell instance = new();

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            root_commands._commands.Clear();
        }

        //----------------------------------------------------------------------------------------------------------

        public Shell()
        {
            status = string.Empty;
            Util.SetFlags(ref flags, Flags.Status, false);
        }

        //----------------------------------------------------------------------------------------------------------

        public override void OnCmdLine(in LineParser line)
        {
            root_commands.OnCmdLine(line);
        }
    }
}