using System;
using System.Collections.Generic;
using UnityEngine;

namespace _TERMINAL_
{
    public sealed partial class Shell : Command
    {
        public readonly struct CommandInfos
        {
            public readonly object owner;
            public readonly string key;
            internal readonly Action onCmd_exe;
            internal readonly Action<LineParser> onCmd_line;
            internal readonly Action<bool> onCmd_bool;
            internal readonly Action<string, LineParser> onCmd_key_line;

            //----------------------------------------------------------------------------------------------------------

            public CommandInfos(in object owner, in string key,
                in Action onCmd_exe = null,
                in Action<bool> onCmd_bool = null,
                in Action<LineParser> onCmd_line = null,
                in Action<string, LineParser> onCmd_key_line = null)
            {
                this.owner = owner;
                this.key = key;
                this.onCmd_exe = onCmd_exe;
                this.onCmd_bool = onCmd_bool;
                this.onCmd_line = onCmd_line;
                this.onCmd_key_line = onCmd_key_line;
            }
        }

        public static readonly Dictionary<object, CommandInfos> _commands = new();

        public static readonly Shell instance = new();

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            _commands.Clear();
        }

        //----------------------------------------------------------------------------------------------------------

        public Shell()
        {
            status = string.Empty;
            Util.SetFlags(ref flags, Flags.Status, false);
        }

        //----------------------------------------------------------------------------------------------------------

        public static void AddCommand(in CommandInfos infos, params object[] aliases)
        {
            _commands.Add(infos.key, infos);
            for (int i = 0; i < aliases.Length; i++)
                _commands.Add(aliases[i], infos);
        }

        public static void RemoveUser(in object owner)
        {
            if (owner == null)
            {
                Debug.LogError($"{typeof(Shell)}.{nameof(RemoveUser)} null {nameof(owner)})");
                return;
            }

            Dictionary<object, CommandInfos> copy = new(_commands);
            _commands.Clear();

            foreach (var pair in copy)
                if (owner != pair.Value.owner)
                    _commands.Add(pair.Key, pair.Value);
        }

        public override void OnCmdLine(in string arg0, in LineParser line)
        {
            if (line.IsCplThis)
                line.OnCpls(arg0, _commands.Keys);
            else if (_commands.TryGetValue(arg0, out var info))
            {
                if (line.IsExec)
                    info.onCmd_exe?.Invoke();

                info.onCmd_line?.Invoke(line);
                info.onCmd_key_line?.Invoke(arg0, line);

                if (info.onCmd_bool != null)
                {
                    bool notEmpty = line.TryRead(out string arg1);
                    if (line.IsCplThis)
                        line.OnCpls(arg1, "TRUE", "FALSE");
                    else if (line.IsExec)
                        if (notEmpty)
                            if (bool.TryParse(arg1, out bool toggle))
                                info.onCmd_bool(toggle);
                            else
                                Debug.LogWarning($"{this} in command \"{arg0}\": could not parse boolean \"{arg1}\"");
                        else
                            Debug.LogWarning($"{this} in command \"{arg0}\": expected boolean");
                }
            }
            else
                base.OnCmdLine(arg0, line);
            line.cmdM |= CmdM._history;
        }
    }
}