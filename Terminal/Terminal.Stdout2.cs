using System.Collections.Generic;
using System.Text;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        readonly Dictionary<object, string> all_stdout2 = new();

        //----------------------------------------------------------------------------------------------------------

        public void OnStdout2(in object key, string value, in bool printOnRemove = false)
        {
            StringBuilder log = new();

            lock (all_stdout2)
            {
                if (value != null)
                    all_stdout2[key] = value;
                else if (all_stdout2.Remove(key, out value))
                    if (printOnRemove)
                        print(value);

                foreach (var pair in all_stdout2)
                    log.AppendLine(pair.Value);
            }

            if (log.Length > 0)
                stdout2.text = log.ToString()[..^1];
            else
                stdout2.text = string.Empty;
        }
    }
}