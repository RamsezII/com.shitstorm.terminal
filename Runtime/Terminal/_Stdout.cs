using _UTIL_;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        public const int MAX_LINES = 256;
        const int MAX_PENDING_LINES = 4096;

        static readonly Queue<string> lines = new();
        static readonly ConcurrentQueue<string> pendingLines = new();
        static int pendingLineCount;

        public static readonly ValueNotifier<Action<string>> onAddLine = new();

        //----------------------------------------------------------------------------------------------------------

        public static void AddLine(in string line)
        {
            int count = Interlocked.Increment(ref pendingLineCount);
            pendingLines.Enqueue(line ?? string.Empty);

            if (count <= MAX_PENDING_LINES)
                return;

            if (pendingLines.TryDequeue(out _))
                Interlocked.Decrement(ref pendingLineCount);
        }

        void DrainPendingLines()
        {
            bool changed = false;
            int drained = 0;

            while (drained++ < MAX_PENDING_LINES && pendingLines.TryDequeue(out string line))
            {
                Interlocked.Decrement(ref pendingLineCount);

                lock (lines)
                {
                    lines.Enqueue(line);
                    while (lines.Count > MAX_LINES)
                        lines.Dequeue();
                }

                onAddLine._value?.Invoke(line);
                changed = true;
            }

            if (!changed)
                return;

            lock (lines)
                stdout1.text = string.Join("\n", lines);

            bottomFlag = true;
        }

        public void ClearLines()
        {
            while (pendingLines.TryDequeue(out _))
                Interlocked.Decrement(ref pendingLineCount);

            lock (lines)
                lines.Clear();

            stdout1.text = string.Empty;
            gui_yscroll = 0;
            bottomFlag = false;
        }

        static void ResetOutput()
        {
            while (pendingLines.TryDequeue(out _))
            {
            }

            Interlocked.Exchange(ref pendingLineCount, 0);

            lock (lines)
                lines.Clear();
        }
    }
}
