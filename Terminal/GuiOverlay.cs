using System;
using UnityEngine;

namespace _TERMINAL_
{
    public class GuiOverlay : MonoBehaviour
    {
        public GUIStyle style;
        public Color backgroundColor = new(0, 0, 0, .95f);
        public Action<GuiOverlay> onGui;

        //--------------------------------------------------------------------------------------------------------------

        public static GuiOverlay Instantiate(in Action<GuiOverlay> onGui)
        {
            GuiOverlay instance = Instantiate(Resources.Load<GuiOverlay>(typeof(GuiOverlay).FullName));
            instance.onGui = onGui;
            return instance;
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        //--------------------------------------------------------------------------------------------------------------

        public void DrawTextSquare(in string text, in bool autoWeight = true)
        {
            Rect rect = new(0, 0, Screen.width, Screen.height);
            if (autoWeight)
                rect.height = style.CalcHeight(new GUIContent(text), rect.width);
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, backgroundColor, 0, 0);
            GUI.Label(rect, text, style);
        }

        protected virtual void OnGUI()
        {
            style.fontSize = Screen.height / 50;
            int padding = Screen.height / 250;
            style.padding = new(2 * padding, 2 * padding, padding, padding);
            onGui?.Invoke(this);
        }
    }
}