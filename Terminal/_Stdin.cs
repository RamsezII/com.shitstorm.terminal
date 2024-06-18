using System;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        void UpdateStdin(in bool ctab, in bool csubmit)
        {
            Event e = Event.current;
            bool uparrow = e.type == EventType.KeyDown && e.keyCode == KeyCode.UpArrow;
            bool downarrow = e.type == EventType.KeyDown && e.keyCode == KeyCode.DownArrow;

            if (uparrow || downarrow)
            {
                if (commands.Count == 1)
                    if (GetHistory(uparrow ? -1 : 1, out string line))
                    {
                        stdin.text = line;
                        RequestCursorMove(line.Length, true);
                    }
                e.Use();
            }
            else if (csubmit || ctab)
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
                    CmdF cmdM = 0;
                    if (csubmit)
                        cmdM |= CmdF.exec;
                    if (ctab)
                        cmdM |= CmdF.tab;

                    if (e.type == EventType.KeyDown && e.alt)
                        if (e.keyCode == KeyCode.UpArrow)
                            cmdM |= CmdF.altN;
                    if (e.keyCode == KeyCode.DownArrow)
                        cmdM |= CmdF.altS;
                    if (e.keyCode == KeyCode.LeftArrow)
                        cmdM |= CmdF.altW;
                    if (e.keyCode == KeyCode.RightArrow)
                        cmdM |= CmdF.altE;

                    LineParser line = new(csubmit ? stdin.text : stdinOld, cmdM, stdinOld.Length);

                    try
                    {
                        string temp = stdin.text;
                        if (csubmit)
                        {
                            string log = command.cmdPrefixe + stdin.text;
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