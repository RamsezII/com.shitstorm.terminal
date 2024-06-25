using System;
using System.Collections.Generic;
using UnityEngine;

namespace _TERMINAL_
{
    public class Commands : Shell.IUser
    {
        public delegate void Action<T>(in T arg);
        public static Commands instance;
        public readonly Dictionary<string, Action<LineParser>> commands = new(StringComparer.OrdinalIgnoreCase);
        IEnumerable<string> Shell.IUser.ECommands => commands.Keys;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init1()
        {
            instance = new Commands();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init2()
        {
            Shell.AddUser(instance);
        }

        //----------------------------------------------------------------------------------------------------------

        void Shell.IUser.OnCmdLine(in string arg0, in LineParser line)
        {
            if (commands.TryGetValue(arg0, out var action))
                action(line);
            else
                Debug.LogError($"{GetType().FullName} Unknown command: \"{arg0}\"");
        }
    }
}