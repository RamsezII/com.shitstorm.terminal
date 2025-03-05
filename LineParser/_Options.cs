using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _TERMINAL_
{
    partial class LineParser
    {
        public bool TryReadFlags(in HashSet<string> flags, out HashSet<string> output)
        {
            output = new(StringComparer.OrdinalIgnoreCase);
            while (TryRead(out string split))
                // args
                if (!split.StartsWith('-'))
                {
                    ReadBack();
                    break;
                }
                // option
                else if (split.StartsWith("--") || split.Length == 1 && IsCplThis)
                {
                    // string
                    string opt = split.TrimStart('-');
                    if (IsCplThis)
                    {
                        OnCpls(opt, flags.Select(x => "--" + x));
                        return true;
                    }
                    else if (flags.Contains(opt))
                    {
                        flags.Remove(opt);
                        output.Add(opt);
                    }
                    else
                    {
                        Debug.LogWarning($"wrong option \"{opt}\"");
                        return false;
                    }
                }
            return true;
        }

        public bool TryReadOptions(in Dictionary<string, Action<string>> options)
        {
            while (TryRead(out string split))
                // args
                if (!split.StartsWith('-'))
                {
                    ReadBack();
                    break;
                }
                // option
                else if (split.StartsWith("--") || split.Length == 1 && IsCplThis)
                {
                    // string
                    string opt = split.TrimStart('-');
                    if (IsCplThis)
                    {
                        OnCpls(opt, options.Keys);
                        return false;
                    }
                    else if (options.TryGetValue(opt, out var action))
                    {
                        if (IsExec)
                            action(opt);
                        options.Remove(opt);
                    }
                    else
                    {
                        Debug.LogWarning($"wrong option \"{opt}\"");
                        return false;
                    }
                }
            return true;
        }
    }
}