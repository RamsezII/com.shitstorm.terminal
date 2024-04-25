using System;
using UnityEngine;

namespace _TERMINAL_
{
    public abstract class Process : IDisposable
    {
        enum Bools : byte
        {
            stdout1,
            stdout2,
            stdin,
            closable,
            killable,
            _last_,
        }

        [Flags]
        public enum Flags : byte
        {
            _none_ = 0,
            Stdout1 = 1 << Bools.stdout1,
            Stdout2 = 1 << Bools.stdout2,
            Stdin = 1 << Bools.stdin,
            Closable = 1 << Bools.closable,
            Killable = 1 << Bools.killable,
            _all_ = (1 << Bools._last_) - 1,
        }

        public string name;
        public Flags flags;

        //----------------------------------------------------------------------------------------------------------

        public Process(in Flags flags, in string name = default)
        {
            this.name = string.IsNullOrWhiteSpace(name) ? GetType().ToString() : name;
            this.flags = flags;
        }

        //----------------------------------------------------------------------------------------------------------

        public void OnCmdLine(in LineParser line) => OnCmdLine(line.Read(), line);
        public virtual void OnCmdLine(in string arg0, in LineParser line) => Debug.LogWarning($"{name} does not implement \"{arg0}\"");
        public virtual void OnGui()
        {
        }

        public virtual void Kill() => Dispose();
        public abstract void Dispose();
    }
}