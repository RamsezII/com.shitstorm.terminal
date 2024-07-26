public static partial class Util_terminal
{
    public static string Quotes(this string str, in bool force)
    {
        if (force || str.Contains(' '))
        {
            bool a = str[0] != '\"';
            bool b = str[^1] != '\"';

            if (a && b)
                str = $"\"{str}\"";
            else if (a)
                str = $"\"{str}";
            else if (b)
                str = $"{str}\"";
        }
        return str;
    }
}