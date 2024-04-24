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
        public static void OnCmdOpts3(LineParser line)
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

            Dictionary<string, object> optsD = new(StringComparer.OrdinalIgnoreCase) { { "port", null }, { "file", null } };
            if (line.TryParseOptions3(OnOpt, optsD) && line.isExec)
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

        public bool TryParseOptions3(TFromUrVo<bool, string, object> TryParse, Dictionary<string, object> optsD)
        {
#if UNITY_EDITOR
            if (optsD.Comparer != StringComparer.OrdinalIgnoreCase)
                Debug.LogWarning($"{nameof(optsD.Comparer)} set to {optsD.Comparer}");
#endif
            bool OnOpt(string read)
            {
                if (TryParse(ref read, out object output))
                    if (optsD.TryGetValue(read, out object already))
                        if (already == null)
                            optsD[read] = output;
                        else
                        {
                            if (isExec)
                                Debug.LogWarning($"<b>{read}</b> already set for <b>{lastRead}</b>");
                            return false;
                        }
                    else
                    {
                        if (isExec)
                            Debug.LogWarning($"{lastRead}: invalid option <b>{read}</b>");
                        return false;
                    }
                return true;
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
                        OnCpls(opt, from pair in optsD where pair.Value == null select pair.Key);
                        return true;
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