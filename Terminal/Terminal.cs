using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal : MonoBehaviour
    {
        public static Terminal terminal;

        float nextCplCheck;
        bool cplFlag, bottomFlag;

        //----------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            name = nameof(Terminal);

            InitGUI();
            ToggleWindow(false);

            font_size = Mathf.Max(15, .02f * Screen.height);
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void OnApplicationFocus(bool focus)
        {
            if (focus)
                ReadHistory("COBRA");
            else
                SaveHistory("COBRA");
        }

        private void OnApplicationQuit()
        {
            SaveHistory("COBRA");
        }

#if UNITY_EDITOR
        [ContextMenu(nameof(_ToggleWindow))]
        void _ToggleWindow() => ToggleWindow(!enabled);
#endif

        public void ToggleWindow(in string stdin)
        {
            this.stdin.text = stdin;
            RequestCursorMove(stdin.Length, true);
            ToggleWindow(true);
        }

        public void ToggleWindow(bool value)
        {
            if (value)
                tryFocus1 = true;
        }

        protected virtual void OnLateUpdate()
        {
            if (cplFlag)
                if (Time.unscaledTime > nextCplCheck)
                {
                    cplFlag = false;
                    RefreshIntell();
                }
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void OnDestroy()
        {
            if (this == terminal)
                terminal = null;

            while (boas.Count > 0)
                boas[^1].Dispose();
            boas.Clear();
        }
    }
}