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
        readonly Queue<string> lines = new();

        //----------------------------------------------------------------------------------------------------------

        public void AddLine(in string line)
        {
            lock (this)
            {
                lines.Enqueue(line);
                while (lines.Count > 50)
                    lines.Dequeue();
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