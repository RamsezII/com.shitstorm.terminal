namespace _TERMINAL_
{
    public partial class LineParser
    {
        public bool Until(in string posM, in string negM) => Until(ref ichar, posM, negM);
        bool Until(ref int ichar, in string posM, in string negM)
        {
            bool
                pos = !string.IsNullOrEmpty(posM),
                neg = !string.IsNullOrEmpty(negM);

            if (!pos && !neg)
            {
                UnityEngine.Debug.LogWarning($"both '{nameof(posM)}' and '{nameof(negM)}' null");
                return false;
            }

            while (ichar < rawtext.Length)
            {
                char c = rawtext[ichar];
                if (pos && posM.Contains(c) || neg && !negM.Contains(c))
                    return true;
                ++ichar;
            }
            return false;
        }
    }
}