using _UTIL_;
using System;
using UnityEngine;

namespace _TERMINAL_
{
    public abstract class Command : IDisposable
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

        public string userName, cmdName, prefixe, status, output;
        public Flags flags = Flags.Stdout1 | Flags.Stdout2 | Flags.Stdin | Flags.Closable;

        public Action onSuccess, onFailure, onDispose;
        public readonly ThreadSafe<bool> disposed = new();

        //----------------------------------------------------------------------------------------------------------

        public Command(in string name = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                cmdName = GetType().ToString();
            else
                cmdName = name;
            RefreshPrefixe();
        }

        public virtual void Init()
        {

        }

        //----------------------------------------------------------------------------------------------------------

        protected void RefreshPrefixe() => prefixe = Terminal.ColoredPrompt(userName, cmdName);

        public void SetUserName(string value)
        {
            userName = value;
            RefreshPrefixe();
        }

        public void SetName(string value)
        {
            cmdName = value;
            RefreshPrefixe();
        }

        //----------------------------------------------------------------------------------------------------------

        public void OnCmdLine(in LineParser line) => OnCmdLine(line.Read(), line);
        public virtual void OnCmdLine(in string arg0, in LineParser line) => Debug.LogWarning($"{cmdName} ({this}) does not implement \"{arg0}\"");
        public virtual void OnGui()
        {
        }

        public void Succeed()
        {
            lock (disposed)
                if (!disposed._value)
                {
                    OnSuccess();
                    onSuccess?.Invoke();
                    Dispose();
                }
        }

        protected virtual void OnSuccess()
        {
        }

        public void Fail()
        {
            lock (disposed)
                if (!disposed._value)
                {
                    OnFailure();
                    onFailure?.Invoke();
                    Dispose();
                }
        }

        protected virtual void OnFailure()
        {
        }

        public void Dispose()
        {
            lock (disposed)
            {
                if (disposed._value)
                    return;
                disposed._value = true;
            }

            if (!string.IsNullOrWhiteSpace(output))
                Debug.Log($"{cmdName} output{{ {output} }}");

            OnDispose();
            onDispose?.Invoke();
        }

        protected virtual void OnDispose()
        {
        }
    }
}