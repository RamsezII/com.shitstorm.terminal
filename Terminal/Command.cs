using _ARK_;
using _UTIL_;
using System;
using System.IO;
using UnityEngine;

namespace _TERMINAL_
{
    public abstract class Command : Disposable, ISchedulable
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

        static readonly string leftPrefixe = Directory.GetCurrentDirectory();
        public readonly string cmdName, cmdPrefixe;

        public string status, output;

        public Flags flags = Flags.Stdout1 | Flags.Stdout2 | Flags.Stdin | Flags.Closable;

        public Action onSuccess, onFailure;

        public bool Scheduled { get; set; }
        public bool Disposed => disposed.Value;

        //----------------------------------------------------------------------------------------------------------

        public Command()
        {
            cmdName = this is Shell ? "~" : GetType().ToString();
            cmdPrefixe = Terminal.ColoredPrompt(leftPrefixe, cmdName);
            status = $"{cmdName}...";
        }

        //----------------------------------------------------------------------------------------------------------

        public virtual void Schedule()
        {
        }

        public virtual void OnTick()
        {
        }

        public void OnCmdLine(in LineParser line) => OnCmdLine(line.Read(), line);
        public virtual void OnCmdLine(in string arg0, in LineParser line)
        {
            if (line.IsExec)
                Debug.LogWarning($"{cmdName} ({this}) does not implement \"{arg0}\"");
        }

        public virtual void OnGui()
        {
        }

        public void Succeed()
        {
            lock (disposed)
                if (!disposed._value)
                {
                    Debug.Log($"----- {cmdName} Success -----");
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
                    Debug.LogWarning($"----- {cmdName} Failure -----");
                    OnFailure();
                    onFailure?.Invoke();
                    Dispose();
                }
        }

        protected virtual void OnFailure()
        {
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (!string.IsNullOrWhiteSpace(output))
                Debug.Log($"{cmdName} output{{ {output} }}");

            Debug.Log($"----- {cmdName} Disposed -----".ToSubLog());
            onDispose?.Invoke();
        }
    }
}