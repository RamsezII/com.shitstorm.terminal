using System;

namespace _TERMINAL_
{
    public class WrongCommandException : Exception
    {
        public WrongCommandException(in string message) : base(message)
        {
        }
    }
}