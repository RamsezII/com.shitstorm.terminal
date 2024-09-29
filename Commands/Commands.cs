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
        static void OnBeforeSceneLoad()
        {
            instance = new Commands();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Shell.AddUser(instance);
        }

        //----------------------------------------------------------------------------------------------------------

        public static void AddCommandKeys(in OnCommand onCommand, params string[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
                instance.commands.Add(keys[i], onCommand);
            Shell.RefreshCommands();
        }

        public static void RemoveKeys(params string[] names)
        {
            foreach (string name in names)
                instance.commands.Remove(name);
            Shell.RefreshCommands();
        }

        public static void RemoveCommand(in OnCommand onCommand)
        {
            HashSet<string> keys = new();

            foreach (var pair in instance.commands)
                if (pair.Value == onCommand)
                    keys.Add(pair.Key);

            foreach (string key in keys)
                instance.commands.Remove(key);

            Shell.RefreshCommands();
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