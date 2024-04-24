using System;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        [Serializable]
        public struct TerminalTheme
        {
            public Color
                backgroundColor,
                color_header_top,
                color_header_bottom;
        }

        [SerializeField] TerminalTheme theme;
        [SerializeField] GUIStyle style_header, style_body;

        //----------------------------------------------------------------------------------------------------------


    }
}