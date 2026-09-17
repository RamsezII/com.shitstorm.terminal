using System.Collections.Generic;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        const int MAX_HISTORY = 100;

        readonly object historyLock = new();
        [SerializeField, UField] List<string> history = new();
        int history_index;

        //----------------------------------------------------------------------------------------------------------

        protected override void OnAfterLoadFields(bool log)
        {
            base.OnAfterLoadFields(log);

            history_index = history.Count;
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

                SaveArkTexts(log: false);
                LoadArkTexts(log: false);
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
