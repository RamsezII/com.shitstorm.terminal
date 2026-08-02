using System.Net;

namespace _TERMINAL_
{
    public partial class LineParser
    {
        public IPEndPoint ReadIpEnd()
        {
            TryReadNetEnd(out IPEndPoint ipEnd);
            return ipEnd;
        }

        public bool TryReadNetEnd(out IPEndPoint result)
        {
            int i = ichar;
            try
            {
                result = new(IPAddress.Parse(Read()), ushort.Parse(Read()));
                return true;
            }
            catch
            {
                ichar = i;
                result = default;
                return false;
            }
        }
    }
}