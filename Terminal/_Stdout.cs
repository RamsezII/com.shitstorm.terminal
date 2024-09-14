using System.Collections.Generic;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        public const int MAX_LINES = 150;
        static readonly Queue<string> lines = new();
        static bool lines_flag;

        //----------------------------------------------------------------------------------------------------------

        public static void AddLine(in string line)
        {
            lock (lines)
            {
                lines_flag = true;
                lines.Enqueue(line);
                while (lines.Count > MAX_LINES)
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