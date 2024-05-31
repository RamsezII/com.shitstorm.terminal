using System;
using System.Collections.Generic;
using System.Linq;

namespace _TERMINAL_
{
    public sealed class Shell : Command
    {
        public interface IUser
        {
            public IEnumerable<string> Commands { get; }
            public void OnCmdLine(in string arg0, in LineParser line);
        }

        public static readonly Shell instance = new();
        static readonly HashSet<IUser> users = new();
        static readonly Dictionary<string, IUser> commandOwners = new(StringComparer.OrdinalIgnoreCase);
        static string[] commands;

        //----------------------------------------------------------------------------------------------------------

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

        public static void AddUser(in IUser user)
        {
            users.Add(user);
            foreach (string cmd in user.Commands)
                commandOwners[cmd] = user;
            RefreshCommands();
        }

        public static void RemoveUser(in IUser user)
        {
            users.Remove(user);
            foreach (string cmd in user.Commands)
                commandOwners.Remove(cmd);
            RefreshCommands();
        }

        public override void OnCmdLine(in string arg0, in LineParser line)
        {
            if (line.isCpl)
                line.OnCpls(arg0, commands);
            else if (commandOwners.TryGetValue(arg0, out IUser user))
                user.OnCmdLine(arg0, line);
            else
                base.OnCmdLine(arg0, line);
        }
    }
}