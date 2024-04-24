using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class LineParser
    {
        public bool TryParseOptions4(HashSet<string> opts_IN, out HashSet<string> opts_OUT)
        {
            var opts_out = opts_OUT = new(StringComparer.OrdinalIgnoreCase);
            bool TryParse(in string opt)
            {
                if (opts_out.Contains(opt))
                {
                    Debug.LogWarning($"\"{opt}\" option is already set");
                    return false;
                }
                else if (opts_IN.Contains(opt))
                    opts_out.Add(opt);
                else
                {
                    Debug.LogWarning($"\"{opt}\" option does not exist");
                    return false;
                }
                return true;
            }

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
                        OnCpls(opt, from opt_in in opts_IN where opt_in.Length > 1 && !opts_out.Contains(opt_in) select opt_in);
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