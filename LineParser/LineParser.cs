using System;

namespace _TERMINAL_
{
    enum CmdB : byte
    {
        exec,
        pipe,
        cpl,
        tab,
        man,
        alt,
        altN,
        altE,
        altS,
        altW,
        _select,
        _applyCpl,
    }

    [Flags]
    public enum CmdM : ushort
    {
        Exec = 1 << CmdB.exec,
        Pipe = 1 << CmdB.pipe,
        Cpl = 1 << CmdB.cpl,
        Tab = 1 << CmdB.tab | Cpl,
        Man = 1 << CmdB.alt,
        Alt = 1 << CmdB.alt | Cpl,
        AltN = 1 << CmdB.altN | Alt,
        AltE = 1 << CmdB.altE | Alt,
        AltS = 1 << CmdB.altS | Alt,
        AltW = 1 << CmdB.altW | Alt,
        AltNS = AltN | AltS,
        AltAll = AltN | AltE | AltS | AltW,
        _select = 1 << CmdB._select,
        _applyCpl = 1 << CmdB._applyCpl,
    }

    [Serializable]
    public sealed partial class LineParser
    {
        public CmdM cmdM;
        public int ichar, ichar_a, ichar_b, sel_move;
        int sel_char;
        public string rawtext = string.Empty;
        public string lastRead;

        public bool IsExec => cmdM.HasFlag(CmdM.Exec);
        public bool IsCpl => cmdM.HasFlag(CmdM.Cpl);
        public bool IsMan => cmdM.HasFlag(CmdM.Man);
        public bool IsThisMan => cmdM.HasFlag(CmdM._select | CmdM.Man);
        public bool IsCplThis => string.IsNullOrWhiteSpace(rawtext) || cmdM.HasFlag(CmdM.Cpl | CmdM._select);

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

        public LineParser(in string rawtext, in CmdM cmdM, in int sel_char = 0)
        {
            this.rawtext = rawtext;
            this.cmdM = cmdM;
            ichar = ichar_a = ichar_b = sel_move = 0;
            this.sel_char = sel_char;
        }
    }
}