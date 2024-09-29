using System.Net;

namespace _TERMINAL_
{
    public partial class LineParser
    {
        public bool GetEveEnd(out IPEndPoint rdvEnd)
        {
            rdvEnd = Util_net.end_ARMA;
            if (TryParseOptions(OptsF.netpoint, out OptsF optsM, out var optsD))
            {
                if (optsM.HasFlag(OptsF.netpoint))
                    rdvEnd = (IPEndPoint)optsD[OptsF.netpoint];
                return true;
            }
            else
                return false;
        }

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