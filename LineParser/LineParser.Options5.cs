using System;
using System.Collections.Generic;
using System.Linq;

namespace _TERMINAL_
{
    public partial class LineParser
    {
        public bool TryParseOptions5(HashSet<string> opts, in Func<string, bool> TryParse)
        {
            while (TryRead(out string split))
            {
                // args
                if (!split.StartsWith('-'))
                {
                    ReadBack();
                    break;
                }
                // option
                else if (split.StartsWith("--") || split.Length == 1 && isCplThis)
                {
                    // string
                    string opt = split.TrimStart('-');
                    if (isCplThis)
                        OnCpls(opt, from optTab in opts where optTab.Length > 1 select optTab);
                    else if (!TryParse(opt))
                        return false;
                }
                else
                    // char
                    for (int i = 1; i < split.Length;)
                        if (!TryParse(split[i..++i]))
                            return false;
            }
            return true;
        }
    }
}