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
            instance = null;
            ResetOutput();
            onAddLine.Reset();
            LineParser.ResetCompletion();
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

            if (EventSystem.current != null)
            {
                memorizedSelection = EventSystem.current.currentSelectedGameObject;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        protected virtual void OnDisable()
        {
            UsageManager.RemoveUser(this);

            if (IMGUI_global.instance != null)
                IMGUI_global.instance.gui_users.RemoveElement(OnOnGui);

            if (memorizedSelection != null)
                NUCLEOR.delegates.Update_OnStartOfFrame_once += () =>
                {
                    if (EventSystem.current != null)
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

        public void CloseAtEndOfFrame() => Util.AddActionOnce(ref NUCLEOR.delegates.LateUpdate_onEndOfFrame_once, ToggleWindowOff);
        public void ToggleWindowOff() => ToggleWindow(false);
        public void ToggleWindow(in bool value)
        {
            if (value)
                tryFocus1 = true;
            else
                stdoutSelectionMode = false;

            enabled = value;
        }

        //----------------------------------------------------------------------------------------------------------

        private void LateUpdate()
        {
            DrainPendingLines();

            lock (commands)
                if (commands.Count > 0 && commands[^1].Disposed)
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

            NUCLEOR.delegates.OnApplicationFocus -= ReadHistory;
            NUCLEOR.delegates.OnApplicationUnfocus -= SaveHistory;

            if (IMGUI_global.instance != null)
            {
                IMGUI_global.instance.inputs_users.RemoveElement(OnOnGuiInputs);
                IMGUI_global.instance.gui_users.RemoveElement(OnOnGui);
            }
        }
    }
}
