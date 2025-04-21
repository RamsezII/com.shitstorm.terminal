using System.Text;

namespace _TERMINAL_
{
    public partial class LineParser
    {
        public bool TryReadAsTime(out float time)
        {
            try
            {
                if (TryRead(out string read))
                {
                    time = ParseTime(read);
                    return true;
                }
                else
                {
                    time = 0;
                    return false;
                }
            }
            catch
            {
                time = 0;
                return false;
            }
        }

        public float ReadAsTime()
        {
            try
            {
                if (TryRead(out string read))
                    return ParseTime(read);
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        public static string LogAsTime(in float time)
        {
            StringBuilder log = new();
            int m = (int)time / 60;
            if (m != 0)
                log.Append(m + "m");
            int s = (int)(time % 60);
            if (s != 0)
                log.Append(s + "s");
            if (m + s == 0)
                log.Append("0s");
            return log.ToString();
        }

        public static float ParseTime(in string read)
        {
            if (read.TryParse(out float time))
                return time;
            else
            {
                int b = read.IndexOf('m'), a = b;
                if (b != -1)
                {
                    time += 60 * read[..b].ToFloat();
                    a = b + 1;
                }
                else
                    a = b = 0;

                b = read.IndexOf('s', b);
                if (b == -1)
                    b = read.Length;

                if (a != b)
                    time += read[a..b].ToFloat();

                return time;
            }
        }
    }
}