using _UTIL_;
using System;

namespace _TERMINAL_
{
    public partial class Boa
    {
        enum FlagsB : byte
        {
            _none_,
            stdout,
            stdin,
            closable,
            running,
            success,
            failure,
            killable,
            killed,
            disposed,
            _last_,
        }

        [Flags]
        public enum FlagsF : ushort
        {
            _none_ = 0,
            Stdout = 1 << FlagsB.stdout,
            Stdin = 1 << FlagsB.stdin,
            Closable = 1 << FlagsB.closable,
            Running = 1 << FlagsB.running,
            Success = 1 << FlagsB.success,
            Failure = 1 << FlagsB.failure,
            Killable = 1 << FlagsB.killable,
            Killed = 1 << FlagsB.killed,
            Disposed = 1 << FlagsB.disposed,
            _all_ = (1 << FlagsB._last_) - 1,
        }

        public readonly ThreadSafe<FlagsF> flags = new(FlagsF.Stdout);

        //----------------------------------------------------------------------------------------------------------

        public void SetFlags(in FlagsF flags) => this.flags.Value |= flags;
        public void UnsetFlags(in FlagsF flags) => this.flags.Value &= ~flags;
        public bool HasFlags(in FlagsF flags) => (this.flags.Value & flags) == flags;
    }
}