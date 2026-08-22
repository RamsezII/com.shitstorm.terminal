using _ARK_;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        static readonly string HISTORY_FILE = typeof(Terminal).FullName + ".history.txt";
        const int MAX_HISTORY = 500;

        private static string GetHistoryPath() => Path.Combine(ArkMachine.DFHome.FullName, HISTORY_FILE);

        readonly object historyLock = new();
        [SerializeField] List<string> history = new();
        int history_index;

        //----------------------------------------------------------------------------------------------------------

        public void SaveHistory()
        {
            string path = GetHistoryPath();
            string[] snapshot;

            lock (historyLock)
                snapshot = history.ToArray();

            try
            {
                File.WriteAllLines(path, snapshot);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Debug.LogWarning($"Could not save terminal history: {ex.Message}");
            }
        }

        public void ReadHistory()
        {
            string path = GetHistoryPath();
            try
            {
                List<string> loaded = File.Exists(path)
                    ? File.ReadAllLines(path).TakeLast(MAX_HISTORY).ToList()
                    : new List<string>();

                lock (historyLock)
                {
                    history = loaded;
                    history_index = history.Count;
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Debug.LogWarning($"Could not read terminal history: {ex.Message}");
            }
        }

        void AddToHistory(in string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;

            lock (historyLock)
            {
                history.Remove(line);
                history.Add(line);
                while (history.Count > MAX_HISTORY)
                    history.RemoveAt(0);
                history_index = history.Count;
            }
        }

        bool GetHistory(in int increment, out string line)
        {
            lock (historyLock)
            {
                if (history.Count == 0)
                {
                    line = null;
                    return false;
                }

                history_index += increment;
                if (history_index < 0)
                    history_index = history.Count - 1;
                else if (history_index > history.Count)
                    history_index = 0;

                if (history_index == history.Count)
                    line = string.Empty;
                else
                    line = history[history_index];

                return true;
            }
        }
    }
}
