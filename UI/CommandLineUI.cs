using _ARK_;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _TERMINAL_
{
    public partial class CommandLineUI : MonoBehaviour, IInputsUser, IMouseUser
    {
        public static CommandLineUI instance;

        [SerializeField] TMP_InputField inputField;
        [SerializeField] RectTransform viewport_rT, content_rT;
        [SerializeField] VerticalLayoutGroup layout;
        [SerializeField] Button button_prefab;

        public void Toggle() => Enabled = !Enabled;
        public bool Enabled
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Util.InstantiateOrCreateIfAbsent<CommandLineUI>();
        }

        //----------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            instance = this;

            inputField = transform.Find("rT/CommandLine_input").GetComponent<TMP_InputField>();
            layout = transform.Find("rT/Scroll View/Viewport/Content/Layout").GetComponent<VerticalLayoutGroup>();
            content_rT = (RectTransform)layout.transform.parent;
            viewport_rT = (RectTransform)content_rT.parent;
            button_prefab = transform.Find("rT/Scroll View/Viewport/Content/Layout/Line").GetComponent<Button>();

            transform.Find("rT/Close").GetComponent<Button>().onClick.AddListener(Toggle);
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void OnEnable()
        {
            NUCLEOR.inputsUsers.Add(this);
            NUCLEOR.mouseUsers.Add(this);
            Shell.commands.AddListener(OnCommands);
        }

        protected virtual void OnDisable()
        {
            NUCLEOR.inputsUsers.Remove(this);
            NUCLEOR.mouseUsers.Remove(this);
            Shell.commands.RemoveListener(OnCommands);
        }

        //----------------------------------------------------------------------------------------------------------

        private void Start()
        {
            button_prefab.gameObject.SetActive(false);
            Enabled = false;
        }

        //----------------------------------------------------------------------------------------------------------

        void OnCommands(string[] commands)
        {
            if (button_prefab == null)
                return;

            ClearButtons();
            float width = 0;
            if (commands != null)
                for (int i = 0; i < commands.Length; i++)
                    width = Mathf.Max(width, AddButton(commands[i]));
            RefreshViewSize(width);
        }

        public void ClearButtons()
        {
            for (int i = 1; i < button_prefab.transform.parent.childCount; i++)
            {
                Transform child = button_prefab.transform.parent.GetChild(i);
                if (child != null)
                    Destroy(child.gameObject);
            }
        }

        public void RefreshViewSize(in float preferredWidth)
        {
            Debug.Log(nameof(RefreshViewSize), this);
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layout.transform);
            float viewportWidth = viewport_rT.rect.size.x;
            content_rT.sizeDelta = new Vector2(Mathf.Max(preferredWidth, viewportWidth), layout.preferredHeight);
        }

        public float AddButton(in string text)
        {
            Button button = Instantiate(button_prefab, button_prefab.transform.parent);
            button.gameObject.SetActive(true);
            TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>();
            float preferredWidth = tmp.GetPreferredValues(text, Mathf.Infinity, Mathf.Infinity).x;
            tmp.text = text;
            button.onClick.AddListener(() => OnClickButton(button));
            return preferredWidth;
        }

        public void OnClickButton(in Button button)
        {
            TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log(tmp.text, button);
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            if (this == instance)
                instance = null;
        }
    }
}
