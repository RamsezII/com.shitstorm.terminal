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

        public const string
            UserColor = "#73CC26",
            BoaColor = "#73B2D9";

        public string userName, cmdName, prefixe;
        public Flags flags = Flags.Stdout1 | Flags.Stdout2 | Flags.Stdin | Flags.Closable;

        //----------------------------------------------------------------------------------------------------------

        public Process(in string name = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                cmdName = GetType().ToString();
            else
                cmdName = name;
            RefreshPrefixe();
        }

        //----------------------------------------------------------------------------------------------------------

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

        public void RefreshPrefixe() => prefixe = $"{userName.SetColor(UserColor)}:{cmdName.SetColor(BoaColor)}$ ";

        //----------------------------------------------------------------------------------------------------------

        public void OnCmdLine(in LineParser line) => OnCmdLine(line.Read(), line);
        public virtual void OnCmdLine(in string arg0, in LineParser line) => Debug.LogWarning($"{cmdName} does not implement \"{arg0}\"");
        public virtual void OnGui()
        {
        }

        public virtual void OnKill() => Dispose();
        public abstract void Dispose();
    }
}