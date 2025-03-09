using System.IO;

namespace _TERMINAL_
{
    partial class LineParser
    {
        public void WriteBytes(in BinaryWriter writer)
        {
            writer.Write(rawtext);
            writer.Write((byte)cmdM);
            writer.Write(ichar);
            writer.Write(ichar_a);
            writer.Write(ichar_b);
            writer.Write(sel_move);
            writer.Write(sel_char);

            //return $"[{rawtext}] mask:{cmdM} {ichar} {ichar_a} {ichar_b} {sel_move} {sel_char}";
        }

        public static LineParser ReadBytes(in BinaryReader reader)
        {
            string rawtext = reader.ReadString();
            CmdM cmdM = (CmdM)reader.ReadByte();
            int ichar = reader.ReadInt32();
            int ichar_a = reader.ReadInt32();
            int ichar_b = reader.ReadInt32();
            int sel_move = reader.ReadInt32();
            int sel_char = reader.ReadInt32();

            LineParser line = new(rawtext, cmdM, sel_char)
            {
                ichar = ichar,
                ichar_a = ichar_a,
                ichar_b = ichar_b,
                sel_move = sel_move
            };

            return line;
        }
    }
}