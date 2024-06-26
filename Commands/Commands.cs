using System;
using System.Collections.Generic;
using UnityEngine;

namespace _TERMINAL_
{
    public class Commands : IShell
    {
        public delegate void OnCommand(in string arg0, in LineParser line);
        public static Commands instance;
        readonly Dictionary<string, OnCommand> commands = new(StringComparer.OrdinalIgnoreCase);
        IEnumerable<string> IShell.ECommands => commands.Keys;

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

        public static void AddCommand(in string name, in OnCommand onCommand) => instance.commands.Add(name, onCommand);
        public static void AddCommands(in OnCommand onCommand, params string[] names)
        {
            for (int i = 0; i < names.Length; i++)
                instance.commands.Add(names[i], onCommand);
        }

        //----------------------------------------------------------------------------------------------------------

        void IShell.OnCmdLine(in string arg0, in LineParser line)
        {
            if (commands.TryGetValue(arg0, out var onCommand))
                onCommand(arg0, line);
            else
                Debug.LogError($"{GetType().FullName} Unknown command: \"{arg0}\"");
        }
    }
}