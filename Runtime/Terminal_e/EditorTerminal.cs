#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace _TERMINAL_.editor
{
    [InitializeOnLoad]
    public sealed partial class EditorTerminalWindow : EditorWindow
    {
        const string
            controlName_inputfield = "TerminalInputField",
            buttonName = nameof(_TERMINAL_) + "/" + nameof(EditorTerminalWindow);

        [SerializeField] string inputText = string.Empty;
        [SerializeField] int flag_focus;
        [SerializeField] int inputControlID;
        [SerializeField] int desiredCursorPos;

        public static readonly Shell.Namespace editor_commands = new(deleteKey: null, comparer: StringComparer.OrdinalIgnoreCase);

        //--------------------------------------------------------------------------------------------------------------

        static EditorTerminalWindow()
        {
            editor_commands._commands.Clear();
        }

        [DidReloadScripts]
        static void OnAfterSceneLoad()
        {
            editor_commands.AddCommand(new(
                deleteKey: null,
                name: "Test",
                onCmd_exe: static () =>
                {
                    Debug.Log("balls");
                }
            ));
        }

        //--------------------------------------------------------------------------------------------------------------

        [MenuItem(buttonName + " %#t")] // Ctrl/Cmd + Shift + T
        [MenuItem("Assets/" + buttonName)]
        static void Open()
        {
            var w = GetWindow<EditorTerminalWindow>();
            w.titleContent = new GUIContent("Terminal");
            w.minSize = new Vector2(300, 60);
            w.flag_focus = 1;
            w.Show();
        }

        private void OnEnable()
        {
            flag_focus = 1;
        }

        private void OnGUI()
        {
            // Enter key should run even if the text field currently has focus.
            var e = Event.current;

            if (e.type == EventType.KeyDown)
                switch (e.keyCode)
                {
                    case KeyCode.Return:
                        OnCommandLine(CmdM.Exec);
                        e.Use();
                        break;

                    case KeyCode.Tab:
                        e.Use();
                        EditorApplication.delayCall += () =>
                        {
                            OnCommandLine(CmdM.Tab);
                        };
                        break;

                    case KeyCode.LeftAlt or KeyCode.RightAlt:
                        e.Use();
                        EditorApplication.delayCall += () =>
                        {
                            OnCommandLine(CmdM.Tab);
                        };
                        return;
                }

            GUILayout.Space(6);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(">", GUILayout.Width(14));

                // Keep focus on the input field
                if (flag_focus > 0)
                    if (--flag_focus == 0)
                    {
                        EditorGUI.FocusTextInControl(controlName_inputfield);
                        var te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), inputControlID);
                        te.text = inputText;
                        te.cursorIndex = te.selectIndex = desiredCursorPos;
                    }

                GUI.SetNextControlName(controlName_inputfield);
                inputText = EditorGUILayout.TextField(inputText);
                if (GUI.GetNameOfFocusedControl() == controlName_inputfield)
                    inputControlID = GUIUtility.keyboardControl;

                bool overlay = false;
                if (overlay)
                {
                    var r = GUILayoutUtility.GetLastRect();
                    var prevc = GUI.color;
                    GUI.color = new Color(1, 1, 1, 0.5f);
                    GUI.Label(r, inputText, EditorStyles.textField);
                    GUI.color = prevc;
                }
            }
        }

        void OnCommandLine(in CmdM options)
        {
            bool submit = options.HasFlag(CmdM.Exec);

            if (submit)
                Debug.Log("> " + inputText);

            LineParser line = new(inputText, Application.dataPath, options, inputText.Length);
            editor_commands.OnCmdLine(line);
            inputText = line.rawtext;

            if (line.IsCpl)
            {
                var te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), inputControlID);
                te.text = line.rawtext;
                desiredCursorPos = te.cursorIndex = te.selectIndex = te.cursorIndex + line.sel_move;
                GUI.changed = true;
                flag_focus = 1;
                Repaint();
            }

            if (submit)
            {
                inputText = string.Empty;
                flag_focus = 1; // refocus after clearing
                Repaint();
            }
        }
    }
}
#endif