using _UTIL_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class LineParser
    {
        public static void OnCmdOpts2(LineParser line)
        {
            bool OnOpt(ref string read, out object result)
            {
                result = null;
                read = read switch
                {
                    "p" or "p1" or "p2" => "port",
                    "f" => "path",
                    _ => read,
                };
                switch (read.ToLower())
                {
                    case "port":
                        if (line.TryRead(out string r1) && ushort.TryParse(r1, out ushort r2))
                            result = r2;
                        else
                            return false;
                        break;

                    case "file":
                    case "path":
                        result = line.ReadAsPath();
                        break;

                    default:
                        return false;
                }
                return true;
            }
            if (line.TryParseOptions2(line.lastRead, new HashSet<string>() { "port", "file" }, OnOpt, out var optsD) && line.isExec)
            {
                StringBuilder log = new();
                foreach (var pair in optsD)
                    log.AppendLine($"{pair.Key} : {pair.Value}");
                if (log.Length == 0)
                    Debug.Log("no options set");
                else
                    Debug.Log(log);
            }
        }

        public bool TryParseOptions2(string arg0, in IEnumerable<string> opts, TFromUrVo<bool, string, object> TryParse, out Dictionary<string, object> optsD)
        {
            var _optsD = optsD = new(StringComparer.OrdinalIgnoreCase);

            bool OnOpt(string read)
            {
                if (TryParse(ref read, out object output))
                    if (_optsD.ContainsKey(read))
                    {
                        if (isExec)
                            Debug.LogWarning($"<b>{read}</b> already set for <b>{arg0}</b>");
                        return false;
                    }
                    else
                    {
                        _optsD[read] = output;
                        return true;
                    }
                else
                {
                    if (isExec)
                        Debug.LogWarning($"{arg0}: invalid option <b>{read}</b>");
                    return false;
                }
            }

            while (TryRead(out string read))
                // args
                if (!read.StartsWith('-'))
                {
                    ichar = ichar_a;
                    break;
                }
                // option
                else if (read.StartsWith("--") || read.Length == 1 && isCplThis)
                {
                    // string
                    string opt = read.TrimStart('-');
                    if (isCplThis)
                    {
                        if (opts != null)
                            OnCpls(opt, from o in opts where !_optsD.ContainsKey(o) select o);
                        return false;
                    }
                    else if (!OnOpt(opt))
                        return false;
                }
                else
                    // char
                    for (int i = 1; i < read.Length; ++i)
                    {
                        string c = read[i].ToString();
                        if (!OnOpt(c))
                            return false;
                    }

            return true;
        }
    }
}