using System;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        void UpdateStdin(in bool ctab, in bool csubmit)
        {
            Event e = Event.current;
            bool upArrow = e.type == EventType.KeyDown && e.keyCode == KeyCode.UpArrow;
            bool downArrow = e.type == EventType.KeyDown && e.keyCode == KeyCode.DownArrow;

            if (!e.alt && (upArrow || downArrow))
            {
                if (commands.Count == 1)
                    if (GetHistory(upArrow ? -1 : 1, out string line))
                    {
                        stdin.text = line;
                        RequestCursorMove(line.Length, true);
                    }
                e.Use();
            }
            else if (e.alt || csubmit || ctab)
            {
                Command command = commands[^1];
                if (csubmit && string.IsNullOrWhiteSpace(stdin.text))
                {
                    stdin.text = string.Empty;
                    if (command.flags.HasFlag(Command.Flags.Closable))
                        ToggleWindow(false);
                }
                else
                {
                    CmdM cmdM = 0;
                    if (csubmit)
                        cmdM |= CmdM.Exec;
                    if (ctab)
                        cmdM |= CmdM.Tab;
                    if (e.alt)
                        cmdM |= CmdM.Alt;

                    if (e.type == EventType.KeyDown)
                    {
                        if (e.keyCode == KeyCode.UpArrow)
                            cmdM |= CmdM.North;
                        if (e.keyCode == KeyCode.RightArrow)
                            cmdM |= CmdM.East;
                        if (e.keyCode == KeyCode.DownArrow)
                            cmdM |= CmdM.South;
                        if (e.keyCode == KeyCode.LeftArrow)
                            cmdM |= CmdM.West;
                    }

                    LineParser line = new(csubmit ? stdin.text : stdinOld, cmdM, stdinOld.Length);

                    try
                    {
                        string temp = stdin.text;
                        if (csubmit)
                        {
                            string log = cmd_prefixe + stdin.text;
                            if (this == instance)
                                print(log);
                            else
                                AddLine(log);
                            stdin.text = string.Empty;
                        }

                        command.OnCmdLine(line);

                        if (csubmit && commands.Count == 1)
                            AddToHistory(temp);

                        if (ctab || line.IsCpl)
                        {
                            stdin.text = line.rawtext;
                            RequestCursorMove(line.sel_move, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }
    }
}