using System.Collections.Generic;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        public const string
            UserColor = "#73CC26",
            BoaColor = "#73B2D9";

        readonly List<Boa> boas = new();

        public string userName;

        //----------------------------------------------------------------------------------------------------------

        public static string Prefixe(in string user, in string boa = "~") => $"{user.SetColor(UserColor)}:{boa.SetColor(BoaColor)}$ ";

        public void AddBoa(in Boa boa)
        {
            lock (this)
            {
                boa.terminal = this;
                if (boas.Contains(boa))
                    Debug.LogError($"Boa \"{boa.title}\" already running.");
                else
                    boas.Add(boa);
            }
        }

        public void RemoveBoa(in Boa boa)
        {
            lock (this)
                boas.Remove(boa);
        }
    }
}