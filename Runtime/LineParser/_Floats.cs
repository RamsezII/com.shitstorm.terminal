namespace _TERMINAL_
{
    public partial class LineParser
    {
        public float ReadFloat() => Read().ToFloat();
        public bool TryReadFloat(out float value)
        {
            value = 0;
            return TryRead(out string read) && read.TryToFloat(out value);
        }
    }
}