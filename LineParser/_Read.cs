namespace _TERMINAL_
{
    public partial class LineParser
    {
        public void ReadBack() => ichar = ichar_a;
        public void ToEnd() => Until(str_stopMask, null);
        public bool TryReadEnd(out string read)
        {
            if (Until(str_stopMask, null))
                switch (rawtext[ichar])
                {
                    case char_pipe2muon:
                        read = str_pipe2muon;
                        ++ichar;
                        return true;

                    case char_pipe2file:
                        read = str_pipe2file;
                        ++ichar;
                        return true;

                    case char_chainCmds:
                        if (rawtext.Length > ichar + 1 && rawtext[ichar + 1] == char_chainCmds)
                        {
                            read = str_chainCmds;
                            ++ichar;
                            ++ichar;
                            return true;
                        }
                        break;
                }
            read = null;
            return false;
        }

        public string ReadAll()
        {
            if (ichar < rawtext.Length)
            {
                string join = rawtext[ichar..];
                ichar = ichar_a = ichar_b = rawtext.Length;
                return join;
            }
            else
                return string.Empty;
        }

        public string Read()
        {
            if (TryRead(out string read))
                return read;
            else
                return string.Empty;
        }

        public bool TryRead(out string result)
        {
            lastRead = result = null;
            ichar_a = ichar;
            if (Until(str_stopMask, " "))
            {
                switch (rawtext[ichar])
                {
                    case char_pipe2file:
                    case char_pipe2muon:
                        return false;
                    case char_chainCmds:
                        if (rawtext.Length > ichar + 1 && rawtext[ichar + 1] == char_chainCmds)
                            return false;
                        break;
                }

                ichar_a = ichar++;
                char ca = rawtext[ichar_a], cb = ca switch
                {
                    '"' or '\'' => ca,
                    '(' => ')',
                    '{' => '}',
                    '[' => ']',
                    _ => '\0',
                };

                if (cb == '\0')
                    Until(" ", null);
                else
                {
                    ++ichar_a;
                    if (!Until(cb.ToString(), null))
                    {
                        UnityEngine.Debug.LogError($"expected: {cb}");
                        lastRead = result = null;
                        return false;
                    }
                }

                ichar_b = ichar++;
                lastRead = result = rawtext[ichar_a..ichar_b];

                if (cb != '\0')
                {
                    --ichar_a;
                    ++ichar_b;
                }

                if (ichar_a <= sel_char && sel_char <= ichar_b)
                    cmdM |= CmdM._select;
                else
                    cmdM &= ~CmdM._select;
                return true;
            }
            else
            {
                ichar_a = ichar_b = ichar;
                if (ichar_a <= sel_char && sel_char <= ichar_b)
                    cmdM |= CmdM._select;
                else
                    cmdM &= ~CmdM._select;
                return false;
            }
        }
    }
}