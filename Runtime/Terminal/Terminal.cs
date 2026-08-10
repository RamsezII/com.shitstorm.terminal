using _ARK_;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _TERMINAL_
{
    public partial class Terminal : MonoBehaviour
    {
        public static Terminal instance;

        public readonly List<Command> commands = new();

        float nextCplCheck;
        bool cplFlag, bottomFlag;

        public bool Enabled
        {
            get => enabled;
            set => ToggleWindow(value);
        }

        public string workdir;

        [SerializeField] GameObject memorizedSelection;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            lines.Clear();
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
            Application.logMessageReceivedThreaded += OnLogMessageReceived;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterLoad()
        {
            Util.InstantiateOrCreateIfAbsent<Terminal>();

            ArkShortcuts.AddShortcut(
                shortcutName: "Terminal",
                action: () => instance.ToggleWindow(true),
                bindings: "p"
            );

            ArkShortcuts.AddShortcut(
                shortcutName: "alt-Terminal",
                action: () => instance.ToggleWindow(true),
                alt: true,
                bindings: "p"
            );
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            instance = this;

            DontDestroyOnLoad(gameObject);

            workdir = ArkMachine.DFHome.FullName;

            InitGUI();
            commands.Add(Shell.instance);
            ReadHistory();
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void OnEnable()
        {
            UsageManager.ToggleUser(this, true, UsageGroups.IMGUI, UsageGroups.TrueMouse, UsageGroups.Keyboard, UsageGroups.Typing, UsageGroups.BlockPlayer);
            IMGUI_global.instance.gui_users.AddElement(OnOnGui);

            memorizedSelection = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(null);
        }

        protected virtual void OnDisable()
        {
            UsageManager.RemoveUser(this);
            IMGUI_global.instance.gui_users.RemoveElement(OnOnGui);

            NUCLEOR.delegates.Update_OnStartOfFrame_once += () =>
            {
                EventSystem.current.SetSelectedGameObject(memorizedSelection);
                memorizedSelection = null;
            };
        }

        //----------------------------------------------------------------------------------------------------------

        private void Start()
        {
            IMGUI_global.instance.inputs_users.AddElement(OnOnGuiInputs);
            ToggleWindow(false);
            NUCLEOR.delegates.OnApplicationFocus += ReadHistory;
            NUCLEOR.delegates.OnApplicationUnfocus += SaveHistory;
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnApplicationQuit() => SaveHistory();

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

        public void ToggleWindow(in bool value)
        {
            if (value)
                tryFocus1 = true;
            enabled = value;
        }

        //----------------------------------------------------------------------------------------------------------

        private void LateUpdate()
        {
            lock (lines)
                if (lines_flag)
                {
                    lines_flag = false;
                    OnAddLine();
                }

            lock (commands)
                if (commands[^1].Disposed)
                {
                    if (commands.Count == 1)
                        Debug.LogError("Main command disposed ???");
                    commands.RemoveAt(commands.Count - 1);
                }

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
            if (this == instance)
                instance = null;

            lock (commands)
            {
                for (int i = commands.Count - 1; i > 0; i--)
                    commands[i].Dispose();
                commands.Clear();
            }

            IMGUI_global.instance.inputs_users.RemoveElement(OnOnGuiInputs);
            IMGUI_global.instance.gui_users.RemoveElement(OnOnGui);
        }
    }
}
