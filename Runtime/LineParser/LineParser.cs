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
        north,
        east,
        south,
        west,
        _select,
        _applyCpl,
        _history,
        _telepathy,
        _last_,
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
        North = 1 << CmdB.north,
        East = 1 << CmdB.east,
        South = 1 << CmdB.south,
        West = 1 << CmdB.west,
        AltNorth = Alt | North,
        AltEast = Alt | East,
        AltSouth = Alt | South,
        AltWest = Alt | West,
        AllDirs = North | East | South | West,
        _select = 1 << CmdB._select,
        _applyCpl = 1 << CmdB._applyCpl,
        _history = 1 << CmdB._history,
        _telepathy = 1 << CmdB._telepathy,
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
        public bool IsCpl => cmdM.HasFlag(CmdM.Tab) | cmdM.HasFlag(CmdM.Alt) & ((cmdM & CmdM.AllDirs) != 0);
        public bool IsMan => cmdM.HasFlag(CmdM.Man);
        public bool IsThisMan => cmdM.HasFlag(CmdM._select | CmdM.Man);
        public bool IsCplThis => string.IsNullOrWhiteSpace(rawtext) || cmdM.HasFlag(CmdM._select) && IsCpl;

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

        public string workdir;

        //----------------------------------------------------------------------------------------------------------

        public LineParser(in string rawtext, in string workdir, in CmdM cmdM, in int sel_char = 0)
        {
            this.rawtext = rawtext;
            this.workdir = workdir;
            this.cmdM = cmdM;
            ichar = ichar_a = ichar_b = sel_move = 0;
            this.sel_char = sel_char;
        }
    }
}