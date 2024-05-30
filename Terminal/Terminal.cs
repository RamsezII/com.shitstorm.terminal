using System.Collections.Generic;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal : MonoBehaviour
    {
        public static Terminal terminal;
        public readonly List<Command> commands = new();

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
            DontDestroyOnLoad(gameObject);

            font_size = Mathf.Max(15, .02f * Screen.height);
        }

        private void Start()
        {
            commands.Add(Shell.instance);
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
            if (commands[^1].disposed.Value)
                commands.RemoveAt(commands.Count - 1);

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

            lock (commands)
            {
                for (int i = commands.Count - 1; i >= 0; i--)
                    commands[i].Dispose();
                commands.Clear();
            }
        }
    }
}