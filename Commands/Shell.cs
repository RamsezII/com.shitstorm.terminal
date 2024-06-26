using System;
using System.Collections.Generic;
using System.Linq;
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
        static string[] commands;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Init()
        {
            users.Clear();
        }

        public Shell()
        {
            status = string.Empty;
        }

        //----------------------------------------------------------------------------------------------------------

        static void RefreshCommands()
        {
            commands = (
                from cmd in commandOwners.Keys
                orderby cmd
                select cmd
                ).ToArray();
        }

        public static void AddUser(in IShell user)
        {
            users.Add(user);
            foreach (string cmd in user.ECommands)
                commandOwners[cmd] = user;
            RefreshCommands();
        }

        public static void RemoveUser(in IShell user)
        {
            users.Remove(user);
            foreach (string cmd in user.ECommands)
                commandOwners.Remove(cmd);
            RefreshCommands();
        }

        public override void OnCmdLine(in string arg0, in LineParser line)
        {
            if (line.IsCplThis)
                line.OnCpls(arg0, commands);
            else if (commandOwners.TryGetValue(arg0, out IShell user))
                user.OnCmdLine(arg0, line);
            else
                base.OnCmdLine(arg0, line);
        }
    }
}