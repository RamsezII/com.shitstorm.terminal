using System;
using System.Collections.Generic;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Boa : IDisposable
    {
        static ushort idCounter;
        public static readonly Dictionary<ushort, Boa> boas = new();

        public readonly ushort bid;
        public Terminal terminal;
        public string title, prefixe = "> ";

        public Action onDispose, onSuccess, onFailure;

        //----------------------------------------------------------------------------------------------------------

        public Boa(in FlagsF flags)
        {
            lock (boas)
            {
                boas.TryReserveKey(ref idCounter, true);
                boas[idCounter] = this;
                bid = idCounter;
            }

            terminal = Terminal.terminal;

            if (flags > 0)
                this.flags._value |= flags;

            terminal.AddBoa(this);
        }

        //----------------------------------------------------------------------------------------------------------

        public virtual void Init()
        {
            title = $"{terminal.name}:{this}";
            flags.Value |= FlagsF.Running;
        }

        //----------------------------------------------------------------------------------------------------------

        public virtual void OnCmdLine(in LineParser line) => OnCmdLine(line.Read(), line);
        protected virtual void OnCmdLine(in string arg0, in LineParser line)
        {
            if (line.isExec)
                throw new WrongCommandException(arg0);
        }

        public virtual void OnGui()
        {
        }

        public void Kill()
        {
            if (!flags.Value.HasFlag(FlagsF.Killed))
            {
                SetFlags(FlagsF.Killed);
                Debug.LogWarning($"aborting command: {GetType().ToString().Bold()}");
                Dispose();
            }
        }

        public void Dispose()
        {
            if (!HasFlags(FlagsF.Disposed))
            {
                OnDispose();
                SetFlags(FlagsF.Disposed);
            }
        }

        protected virtual void OnDispose()
        {
            terminal.OnStdout2(this, null, true);
            terminal.RemoveBoa(this);
            onDispose?.Invoke();

            lock (boas)
                boas.Remove(bid);
        }

        public virtual void OnSuccess()
        {
            if (!HasFlags(FlagsF.Success))
            {
                SetFlags(FlagsF.Success);
                onSuccess?.Invoke();
                Dispose();
            }
        }

        public virtual void OnFailure()
        {
            if (!HasFlags(FlagsF.Failure))
            {
                SetFlags(FlagsF.Failure);
                Debug.LogWarning(GetType() + " failed");
                onFailure?.Invoke();
            }
        }
    }
}