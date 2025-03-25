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

        public bool TryReadOption(in string option, out bool confirmed)
        {
            while (TryRead(out string split))
                if (split.StartsWith('-'))
                {
                    string opt = split.Trim();
                    if (IsCplThis)
                    {
                        if (!OnCpls(opt, option))
                        {
                            ReadBack();
                            confirmed = false;
                            return false;
                        }
                        confirmed = true;
                        return true;
                    }
                    else if (opt.Equals(option, StringComparison.OrdinalIgnoreCase))
                    {
                        confirmed = true;
                        return true;
                    }
                    else
                    {
                        ReadBack();
                        confirmed = false;
                        return true;
                    }
                }
                else
                {
                    ReadBack();
                    confirmed = false;
                    return false;
                }
            confirmed = false;
            return true;
        }

        public bool TryReadOptions(in Dictionary<string, Action<LineParser, string>> onOptions)
        {
            while (TryRead(out string split))
                if (split.StartsWith('-'))
                {
                    string opt = split.Trim();
                    if (IsCplThis)
                    {
                        OnCpls(opt, onOptions.Keys.Where(o => o.StartsWith("--", StringComparison.OrdinalIgnoreCase)));
                        return false;
                    }
                    else if (onOptions.TryGetValue(opt, out var onOption))
                    {
                        onOption(this, opt.TrimStart('-'));
                        onOptions.Remove(opt);
                    }
                    else
                    {
                        Debug.LogWarning($"wrong option \"{opt}\"");
                        return false;
                    }
                }
                else
                {
                    ReadBack();
                    break;
                }
            return true;
        }
    }
}