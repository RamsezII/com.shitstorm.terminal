using System.Collections.Generic;
using System.Linq;

namespace _TERMINAL_
{
    public partial class LineParser
    {
        public const string str_delimiters = "\"'(){}[]";

        //----------------------------------------------------------------------------------------------------------

        public bool OnCpl_solo(in string prefixe, in string cpl)
        {
            tab_last = 0;
            if (string.IsNullOrWhiteSpace(prefixe) || Util.IsMatchChars(prefixe, cpl))
            {
                ReplaceSplit(cpl);
                return true;
            }
            else
                return false;
        }

        public bool OnCpls<T>(in string prefixe, IEnumerable<T> cpls)
        {
            if (!string.IsNullOrWhiteSpace(prefixe))
                cpls = Util.EMatchChars(prefixe, cpls);
            string[] arr = (from cpl in cpls select cpl.ToString()).ToArray();

            if (arr.Length == 0)
            {
                tab_last = 0;
                return false;
            }
            else
            {
                if (cmdM.HasFlag(CmdF.altN))
                    if (tab_last > 0)
                        tab_last = (tab_last - 1) % arr.Length;
                    else
                        tab_last = arr.Length - 1;
                else if (cmdM.HasFlag(CmdF.tab) || cmdM.HasFlag(CmdF.altS))
                    tab_last = (tab_last + 1) % arr.Length;

                ReplaceSplit(arr[tab_last].ToString());
                return true;
            }
        }

        public void ReplaceSplit(string item)
        {
            CheckChain(ref item);
            rawtext = rawtext[..ichar_a] + item + rawtext[ichar_b..];
            sel_move += ichar_a + item.Length - sel_char;
            if (sel_char > rawtext.Length)
                sel_move += sel_char - rawtext.Length;
            ichar = rawtext.Length;
        }

        public static bool CheckChain(ref string chain)
        {
            if (chain.Contains(' ') && !chain.Contains2(str_delimiters))
            {
                chain = "\"" + chain + "\"";
                return true;
            }
            else
                return false;
        }
    }
}