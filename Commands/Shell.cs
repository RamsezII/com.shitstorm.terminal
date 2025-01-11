using _UTIL_;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _TERMINAL_
{
    public static class Util_shell
    {
        public static void OnCmdLine(this IShell user, in LineParser line)
        {
            user.OnCmdLine(line.Read(), line);
        }
    }

    public interface IShell
    {
        public IEnumerable<string> ECommands { get; }
        public void OnCmdLine(in string arg0, in LineParser line);
    }

    public sealed class Shell : Command
    {
        public static readonly Shell instance = new();
        static readonly HashSet<IShell> users = new();
        static readonly Dictionary<string, IShell> commandOwners = new(StringComparer.OrdinalIgnoreCase);
        public static readonly OnValue<string[]> commands = new();

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            users.Clear();
            commands.Reset();
        }

        //----------------------------------------------------------------------------------------------------------

        public Shell()
        {
            status = string.Empty;
            Util.SetFlags(ref flags, Flags.Status, false);
        }

        //----------------------------------------------------------------------------------------------------------

        public static void RefreshCommands()
        {
            commandOwners.Clear();

            List<string> cmds = new();

            foreach (IShell user in users)
                foreach (string cmd in user.ECommands)
                    switch (cmd)
                    {
                        case "_last_":
                        case "_none_":
                        case "_all_":
                        case "_first_":
                            break;
                        default:
                            cmds.Add(cmd);
                            if (!commandOwners.TryAdd(cmd, user))
                                Debug.LogWarning($"Conflict for command \"{cmd}\" between {commandOwners[cmd].GetType().FullName} and {user.GetType().FullName}");
                            break;
                    }

            cmds.Sort();

            commands.Update(cmds.ToArray());
        }

        public static void AddUser(in IShell user)
        {
            users.Add(user);
            RefreshCommands();
        }

        public static void RemoveUser(in IShell user)
        {
            users.Remove(user);
            RefreshCommands();
        }

        public override void OnCmdLine(in string arg0, in LineParser line)
        {
            if (line.IsCplThis)
                line.OnCpls(arg0, commands._value);
            else if (commandOwners.TryGetValue(arg0, out IShell user))
                user.OnCmdLine(arg0, line);
            else
                base.OnCmdLine(arg0, line);
        }
    }
}