using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class LineParser
    {
        public const OptsF opts_readM = OptsF.path | OptsF.pattern | OptsF.regex | OptsF.message | OptsF.netid | OptsF.mid | OptsF.netpoint | OptsF.ip | OptsF.port | OptsF.lifeTime;
        public static readonly IEnumerable<OptsB> eOptsB = from i in Enumerable.Range(0, (int)OptsB._last_) select (OptsB)i;

        //----------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/" + nameof(_TERMINAL_) + "/" + nameof(LogOptsF) + "()")]
        static void LogOptsF()
        {
            StringBuilder log = new();
            for (OptsB optB = 0; optB < OptsB._last_; ++optB)
                log.AppendLine($"{optB} = 1 << {nameof(OptsB)}.{optB},");
            Debug.Log(log);
        }
#endif

        //----------------------------------------------------------------------------------------------------------

        public static void OnCmdOpts(in LineParser line)
        {
            if (line.TryParseOptions(OptsF._all_, out OptsF optsM, out var optsD))
                if (line.isExec)
                {
                    StringBuilder log = new();

                    log.AppendLine($"options: {{ {optsM} }}");

                    if (optsD != null)
                        foreach (var pair in optsD)
                            log.AppendLine($"{pair.Key}: {pair.Value}");

                    log.Append("args: { ");
                    while (line.TryRead(out string read))
                        log.Append($"{read}, ");
                    log.Append("}");

                    Debug.Log(log);
                }
        }

        public bool TryParseNoOptions()
        {
            int i = ichar;
            if (Until(ref i, null, " "))
                return rawtext[i] != '-';
            return true;
        }

        public bool TryParseOptions(OptsF filter, out OptsF optsM, out Dictionary<OptsF, object> optsD)
        {
            optsM = 0;
            optsD = null;

            bool ParseOpt(in OptsF optF, ref OptsF optsM, ref Dictionary<OptsF, object> optsD)
            {
                if (!filter.HasFlag(optF))
                {
                    Debug.LogWarning($"'{optF}' can not be set for this command");
                    return false;
                }
                else if (optsM.HasFlag(optF))
                {
                    Debug.LogWarning($"'{optF}' already set for this command");
                    return false;
                }
                else
                {
                    optsM |= optF;
                    if (opts_readM.HasFlag(optF))
                    {
                        if (isCplThis)
                        {
                            string read = Read();
                            switch (optF)
                            {
                                case OptsF.path:
                                    if (CplPath(this, read, out string result))
                                        ReplaceSplit(result);
                                    break;
                                case OptsF.mid:
                                    break;
                            }
                        }
                        else
                        {
                            optsD ??= new();
                            optsD[optF] = optF switch
                            {
                                OptsF.lifeTime => int.Parse(Read()),
                                OptsF.netid => byte.Parse(Read()),
                                OptsF.port or OptsF.mid => ushort.Parse(Read()),
                                OptsF.netpoint => ReadIpEnd(),
                                OptsF.player => byte.Parse(Read()),
                                _ => Read(),
                            };
                        }
                    }
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
                    OptsF optsM2 = optsM;
                    var query =
                        from optF in
                            from i in Enumerable.Range(0, (int)OptsB._last_)
                            select (OptsF)(1 << i)
                        where filter.HasFlag(optF) && !optsM2.HasFlag(optF)
                        select "--" + optF;

                    if (isCplThis)
                    {
                        OnCpls(opt, query);
                        return false;
                    }
                    else if (!Enum.TryParse(opt, true, out OptsF optF))
                    {
                        Debug.LogWarning($"\"{opt}\" option does not exist");
                        return false;
                    }
                    else if (!ParseOpt(optF, ref optsM, ref optsD))
                        return false;
                }
                else
                    // char
                    for (int i = 1; i < split.Length; ++i)
                    {
                        char opt = split[i];
                        if (!Enum.TryParse(opt.ToString(), false, out OptsF2 optF2))
                        {
                            Debug.LogWarning($"'{opt}' option does not exist");
                            return false;
                        }
                        else if (!ParseOpt((OptsF)optF2, ref optsM, ref optsD))
                            return false;
                    }
            }

            return true;
        }
    }
}