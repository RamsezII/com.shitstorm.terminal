using UnityEngine;

namespace _TERMINAL_
{
    public abstract partial class Shell : Process
    {
        public Shell(in string name = "~") : base(Flags.Stdout1 | Flags.Stdout2 | Flags.Stdin, name)
        {
        }
    }
}