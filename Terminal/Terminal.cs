using System.Collections.Generic;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal : MonoBehaviour
    {
        public static Terminal terminal;
        public readonly List<Process> processes = new();

        float nextCplCheck;
        bool cplFlag, bottomFlag;

        //----------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            name = nameof(Terminal);

            InitGUI();
            ToggleWindow(false);

            font_size = Mathf.Max(15, .02f * Screen.height);

            _ARK_.NUCLEOR.onLateUpdate -= UpdateInputs;
            _ARK_.NUCLEOR.onLateUpdate += UpdateInputs;
        }

        private void Start()
        {
            ToggleWindow(false);
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

        //----------------------------------------------------------------------------------------------------------

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

            _ARK_.NUCLEOR.onLateUpdate -= UpdateInputs;

            lock (processes)
            {
                for (int i = processes.Count - 1; i >= 0; i--)
                    processes[i].Dispose();
                processes.Clear();
            }
        }
    }
}