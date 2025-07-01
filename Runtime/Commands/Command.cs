using _ARK_;
using System;
using UnityEngine;

namespace _TERMINAL_
{
    public abstract class Command : Schedulable
    {
        enum Bools : byte
        {
            stdout,
            status,
            stdin,
            closable,
            killable,
            _last_,
        }

        [Flags]
        public enum Flags : byte
        {
            _none_ = 0,
            Stdout = 1 << Bools.stdout,
            Status = 1 << Bools.status,
            Stdin = 1 << Bools.stdin,
            Closable = 1 << Bools.closable,
            Killable = 1 << Bools.killable,
            _all_ = (1 << Bools._last_) - 1,
        }

        public string cmdName;

        public string status, output;

        public Flags flags = Flags.Stdout | Flags.Status | Flags.Stdin | Flags.Closable;

        public Action onSuccess, onFailure;
        bool logs;

        //----------------------------------------------------------------------------------------------------------

        public Command()
        {
            cmdName = this is Shell ? NUCLEOR.terminal_path : GetType().FullName;
            status = $"{cmdName}...";
        }

        //----------------------------------------------------------------------------------------------------------

        public virtual void OnCmdLine(in LineParser line) => OnCmdLine(line.Read(), line);
        public virtual void OnCmdLine(in string arg0, in LineParser line)
        {
            if (line.IsExec)
                Debug.LogWarning($"{cmdName} ({this}) does not implement \"{arg0}\"");
        }

        public virtual void OnGui()
        {
        }

        public void Succeed(in bool logs)
        {
            this.logs = logs;
            lock (this)
                if (!_disposed)
                {
                    if (this.logs)
                        Debug.Log($"----- {cmdName} Success -----");
                    OnSuccess();
                    onSuccess?.Invoke();
                    Dispose();
                }
        }

        protected virtual void OnSuccess()
        {
        }

        public void Kill()
        {
            lock (this)
                if (!_disposed)
                {
                    Debug.LogWarning($"----- {cmdName} Killed -----");
                    Dispose();
                }
        }

        protected virtual void OnFailure()
        {
            Debug.LogWarning($"----- {cmdName} Failure -----");
            Dispose();
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (Terminal.instance != null)
                Terminal.instance.commands.Remove(this);

            if (!string.IsNullOrWhiteSpace(output))
                Debug.Log($"{cmdName} output{{ {output} }}");

            if (logs)
                Debug.Log($"----- {cmdName} Disposed -----".ToSubLog());
            onDispose?.Invoke();
        }
    }
}