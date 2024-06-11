using _UTIL_;
using System.Collections.Generic;
using UnityEngine;

namespace _TERMINAL_
{
    public enum LogTypes : byte
    {
        Error = LogType.Error,
        Assert = LogType.Assert,
        Warning = LogType.Warning,
        Log = LogType.Log,
        Exception = LogType.Exception,
        SubLog,
    }

    public partial class Terminal
    {
        static readonly Queue<string> lines = new();
        static bool lines_flag;

        //----------------------------------------------------------------------------------------------------------

        public static void AddLine(in string line)
        {
            lock (lines)
            {
                lines_flag = true;
                lines.Enqueue(line);
                while (lines.Count > 50)
                    lines.Dequeue();
            }
        }

        void OnAddLine()
        {
            lock (this)
            {
                stdout1.text = string.Join("\n", lines);
                bottomFlag = true;
            }
        }

        public void ClearLines()
        {
            lock (this)
            {
                lines.Clear();
                stdout1.text = string.Empty;
            }
        }
    }
}