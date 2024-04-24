using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class LineParser
    {
        public bool TryParseOptions6(in Func<string, bool> TryParse, IEnumerable<string> cpls)
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
                else if (split.Length > 2 && split.StartsWith("--") || split.Length == 1 && isCplThis)
                {
                    // string
                    string opt = split.TrimStart('-');
                    if (isCplThis && cpls != null)
                        OnCpls(opt, from optTab in cpls select optTab);
                    else if (!TryParse(opt))
                    {
                        Debug.LogWarning($"\"{opt}\" option can not be set for this command");
                        return false;
                    }
                }
                else
                    // option char
                    for (int i = 1; i < split.Length;)
                    {
                        string opt = split[i..++i];
                        if (!TryParse(opt))
                        {
                            Debug.LogWarning($"\'{opt}\' option can not be set for this command");
                            return false;
                        }
                    }
            }
            return true;
        }
    }
}