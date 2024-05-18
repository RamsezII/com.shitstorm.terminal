using System.Collections.Generic;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal : MonoBehaviour
    {
        public static Terminal terminal;
        public static Process mainProcess;
        public readonly List<Process> processes = new();

        float nextCplCheck;
        bool cplFlag, bottomFlag;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoSpawn()
        {
            if (FindObjectOfType<Terminal>() == null)
                Instantiate(Resources.Load<Terminal>(nameof(_TERMINAL_)));
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            name = nameof(Terminal);
            terminal = this;

#if UNITY_EDITOR
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
#endif
            Application.logMessageReceivedThreaded += OnLogMessageReceived;

            InitGUI();
            ToggleWindow(false);

            font_size = Mathf.Max(15, .02f * Screen.height);
        }

        private void Start()
        {
            if (mainProcess != null)
                processes.Add(mainProcess);
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
            enabled = value;
        }

        //----------------------------------------------------------------------------------------------------------

        private void LateUpdate()
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
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;

            if (this == terminal)
                terminal = null;

            lock (processes)
            {
                for (int i = processes.Count - 1; i >= 0; i--)
                    processes[i].Dispose();
                processes.Clear();
            }
        }
    }
}