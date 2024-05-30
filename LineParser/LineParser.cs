using System;

namespace _TERMINAL_
{
    enum CmdB : byte
    {
        exec,
        pipe,
        cpl,
        tab,
        alt,
        altN,
        altE,
        altS,
        altW,
        _select,
        _applyCpl,
    }

    [Flags]
    public enum CmdF : ushort
    {
        exec = 1 << CmdB.exec,
        pipe = 1 << CmdB.pipe,
        cpl = 1 << CmdB.cpl,
        tab = 1 << CmdB.tab | cpl,
        alt = 1 << CmdB.alt | cpl,
        altN = 1 << CmdB.altN | alt,
        altE = 1 << CmdB.altE | alt,
        altS = 1 << CmdB.altS | alt,
        altW = 1 << CmdB.altW | alt,
        altNS = altN | altS,
        altAll = altN | altE | altS | altW,
        _select = 1 << CmdB._select,
        _cplThis = cpl | _select,
        _tabThis = tab | _select,
        _altThis = alt | _select,
        _applyCpl = 1 << CmdB._applyCpl,
    }

    [Serializable]
    public sealed partial class LineParser
    {
        public CmdF cmdM;
        public int ichar, ichar_a, ichar_b, sel_move;
        int sel_char;
        public string rawtext = string.Empty;
        public string lastRead;

        public bool isExec => cmdM.HasFlag(CmdF.exec);
        public bool isCpl => cmdM.HasFlag(CmdF.cpl);
        public bool isCplThis => string.IsNullOrWhiteSpace(rawtext) || cmdM.HasFlag(CmdF._cplThis);

        const char
            char_chainCmds = '&',
            char_pipe2muon = '|',
            char_pipe2file = '>';

        const string
            str_chainCmds = "&&",
            str_pipe2muon = "|",
            str_pipe2file = ">";

        static readonly string
            str_stopMask = "" + char_chainCmds + char_pipe2muon + char_pipe2file;

        public static int tab_last;

        //----------------------------------------------------------------------------------------------------------

        public LineParser(in string rawtext, in CmdF cmdM, in int sel_char = 0)
        {
            this.rawtext = rawtext;
            this.cmdM = cmdM;
            ichar = ichar_a = ichar_b = sel_move = 0;
            this.sel_char = sel_char;
        }
    }
}