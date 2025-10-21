using _ARK_;
using System;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        [Serializable]
        public struct Text
        {
            public bool enabled;
            public string controlName;
            [TextArea(minLines: 0, maxLines: 5)] public string text;
            public float height;
        }

        [Header("~@ GUI @~")]
        [SerializeField] bool showFilesButton;
        [SerializeField] float font_size = 12;
        [SerializeField] float line_height;

        public Rect window_r = new(100, 50, 350, 500);
        [SerializeField] Rect drag_r;

        [Header("~ Header ~")]
        [SerializeField] float header_height;
        [SerializeField] Texture header_image;

        [Header("~ Body ~")]
        [SerializeField] int margin = 5;
        [SerializeField] float gui_yscroll;
        [SerializeField] Text stdout1, stdout2;
        public Text stdin;

        public bool tryFocus1;
        [SerializeField] bool tryFocus2;

        const int scrollCollumnWidth = 16;
        const float border_radius = 3;

        [SerializeField] string stdinOld;
        [SerializeField] Rect dims_r = new(.1f, .05f, .8f, .85f);

        public bool fullscreen;
        [SerializeField] string cmd_prefixe;

        static readonly bool block_when_nucleor = false;

        //----------------------------------------------------------------------------------------------------------

        void InitGUI()
        {
            font_size = Mathf.Max(15, .01f * Screen.height);
            stdout1.controlName = nameof(stdout1);
            stdout2.controlName = nameof(stdout2);
            stdin.controlName = nameof(stdin);
        }

        //----------------------------------------------------------------------------------------------------------

        bool OnOnGuiInputs(Event e)
        {
            if (e.type == EventType.KeyDown)
            {
                if (Enabled)
                    if (e.keyCode == KeyCode.Return)
                        if (string.IsNullOrWhiteSpace(stdin.text))
                        {
                            stdin.text = string.Empty;
                            if (commands[^1].flags.HasFlag(Command.Flags.Closable))
                                ToggleWindow(false);
                            return true;
                        }

                if (!Enabled)
                    if (e.keyCode == KeyCode.P)
                        if (e.alt || UsageManager.AllAreEmpty(UsageGroups.Typing))
                        {
                            ToggleWindow(true);
                            return true;
                        }
            }
            return false;
        }

        bool OnOnGui(Event e)
        {
            Command command;

            try
            {
                command = commands[^1];
            }
            catch
            {
                return false;
            }

            CheckCursorMoveRequest();

            if (e.type == EventType.ScrollWheel)
                if (e.control)
                {
                    font_size = Mathf.Clamp(font_size - e.delta.y, 5, byte.MaxValue);
                    e.Use();
                }

            style_body.fontSize = (int)(font_size * Screen.height * .001f);

            line_height = style_body.lineHeight;
            header_height = 25;

            window_r = new Rect(
                dims_r.x * Screen.width,
                dims_r.y * Screen.height,
                dims_r.width * Screen.width,
                dims_r.height * Screen.height
                );

            Rect header_r = new(window_r.x, window_r.y, window_r.width, header_height);
            Rect body_r = new(window_r.x, window_r.y + header_height, window_r.width, window_r.height - header_height);
            Rect text_r = new(margin, 0, body_r.width - scrollCollumnWidth - 2 * margin, body_r.height);

            if (fullscreen)
            {
                window_r = new Rect(0, 0, Screen.width, Screen.height);
                header_r = new Rect(0, 0, Screen.width, 0);
                body_r = new Rect(0, 0, Screen.width, Screen.height);
                text_r = new Rect(margin, 0, body_r.width - scrollCollumnWidth - 2 * margin, body_r.height);
            }

            GUI.DrawTexture(window_r, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, theme.backgroundColor, 0, border_radius);

            if (e.type == EventType.MouseDown)
            {
                Vector2 mousePos = Input.mousePosition;
                mousePos.y = Screen.height - mousePos.y;

                if (window_r.Contains(mousePos))
                    tryFocus1 = true;
            }

            if (!fullscreen)
            {
                GUI.DrawTexture(header_r, Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 0, theme.color_header_bottom, default, border_radius * new Vector4(1, 1, 0, 0));
                GUI.DrawTexture(header_r, header_image, ScaleMode.StretchToFill, true, 0, theme.color_header_top, default, border_radius * new Vector4(1, 1, 0, 0));
                GUI.DrawTexture(window_r, Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 0, Color.black, 2, border_radius);
            }

            if (showFilesButton)
            {
                style_header.alignment = TextAnchor.MiddleLeft;
                GUI.Label(new(header_r.x + 5, header_r.y, header_r.width, header_r.height), "Files", style_header);
            }

            if (!fullscreen)
            {
                style_header.alignment = TextAnchor.MiddleCenter;
                GUI.Label(header_r, "TERMINAL", style_header);
            }

            stdout1.height = stdout2.height = stdin.height = 0;

            if (stdout1.enabled = command.flags.HasFlag(Command.Flags.Stdout))
                GetSize(ref stdout1);

            if (NUCLEOR.instance.sequencer.list.IsNotEmpty || command.flags.HasFlag(Command.Flags.Status))
            {
                if (NUCLEOR.instance.sequencer.list.IsNotEmpty)
                {
                    Schedulable schedulable = NUCLEOR.instance.sequencer.list._collection[0];
                    stdout2.text = $"{typeof(NUCLEOR).FullName}({Util_ark.GetRotator()})\n{schedulable.description}\n{schedulable.progressBar}".SetColor(Colors.cyan);
                }
                else
                    stdout2.text = $"---- {command.status} ---- {Util_ark.GetRotator()}";

                stdout2.enabled = !string.IsNullOrWhiteSpace(stdout2.text);
                if (stdout2.enabled)
                    GetSize(ref stdout2);
            }

            if (command.flags.HasFlag(Command.Flags.Stdin))
            {
                GetSize(ref stdin);
                stdin.height = Mathf.Max(stdin.height, line_height);
            }

            Rect scroll_r = new(0, 0, body_r.width - scrollCollumnWidth, stdout1.height + stdout2.height + stdin.height + body_r.height - line_height);
            text_r.height = scroll_r.height;

            float yscroll = GUI.BeginScrollView(
                body_r,
                new(0, gui_yscroll),
                scroll_r,
                false,
                true,
                GUI.skin.horizontalScrollbar,
                GUI.skin.verticalScrollbar
                ).y;

            if (!e.control && !e.alt)
                if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
                    gui_yscroll = yscroll;

            CatchTabAndEnter(false, out bool downTab, out bool downSubmit);

            if (this == null)
                Debug.LogWarning("this==null");

            float text_h = 0;

            if (stdout1.enabled && stdout1.height > 0)
                DrawText(ref stdout1, ref text_h);

            if (stdout2.enabled && stdout2.height > 0)
                DrawText(ref stdout2, ref text_h);

            if (bottomFlag)
            {
                bottomFlag = false;
                gui_yscroll = Mathf.Max(gui_yscroll, text_h - body_r.height + stdin.height + .5f * line_height);
            }

            if (command.flags.HasFlag(Command.Flags.Stdin))
            {
                cmd_prefixe = ColoredPrompt(ArkMachine.user_name.Value, command.cmdName);
                Vector2 prefixe_size = style_body.CalcSize(new(cmd_prefixe));
                GUI.Label(new Rect(text_r.x, text_r.y + text_h, text_r.width, prefixe_size.y), cmd_prefixe, style_body);
                text_r.x += prefixe_size.x;
                text_r.width -= .5f * prefixe_size.x;
            }

            if (e.type == EventType.KeyDown)
                if (e.control)
                    if (e.keyCode == KeyCode.C)
                    {
                        if (command.flags.HasFlag(Command.Flags.Status))
                            Debug.Log($"---- {command.status} ----");

                        Debug.Log("^C");

                        if (command.flags.HasFlag(Command.Flags.Killable))
                        {
                            command.Kill();
                            Event.current.Use();
                        }
                        else
                            Debug.LogWarning($"can not abort \"{command.GetType()}\"");
                    }

            command.OnGui();

            if (command.flags.HasFlag(Command.Flags.Stdin) && !block_when_nucleor || NUCLEOR.instance.sequencer.list.IsEmpty)
            {
                UpdateStdin(downTab, downSubmit);
                stdin.text = ModifyText(ref stdin, ref text_h).Replace("\n", string.Empty);

                if (GUI.changed)
                {
                    stdinOld = stdin.text;
                    bottomFlag = true;
                }
            }
            else
            {
                string temp = stdin.text;
                stdin.text = string.Empty;
                ModifyText(ref stdin, ref text_h);
                stdin.text = temp;
            }

            if (tryFocus2)
            {
                tryFocus2 = false;
                GUI.FocusControl(stdin.controlName);
            }

            if (tryFocus1)
            {
                tryFocus1 = false;
                tryFocus2 = true;
            }

            string nameOfFocusedControl = GUI.GetNameOfFocusedControl();
            bool focused = stdin.controlName.Equals(nameOfFocusedControl, StringComparison.OrdinalIgnoreCase);

            GUI.EndScrollView();

            return false;

            void GetSize(ref Text text)
            {
                if (string.IsNullOrWhiteSpace(text.text))
                    text.height = 0;
                else
                    text.height = style_body.CalcHeight(new(text.text), text_r.width);
                text.enabled = text.height > 0;
            }

            void DrawText(ref Text text, ref float text_h)
            {
                //GUI.SetNextControlName(text.controlName);
                GUI.Label(new Rect(text_r.x, text_r.y + text_h, text_r.width, text.height), text.text, style_body);
                text_h += text.height;
            }

            string ModifyText(ref Text text, ref float text_h)
            {
                GUI.SetNextControlName(text.controlName);

                Rect rect = new(
                    text_r.x, text_r.y + text_h,
                    text_r.width, text.height + line_height
                    );

                string output = text.text;

                try
                {
                    output = GUI.TextArea(rect, text.text, style_body);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"terminal weird shit.. ({ex.TrimmedExceptionMessage()})");
                }

                text_h += text.height;

                return output;
            }
        }
    }
}