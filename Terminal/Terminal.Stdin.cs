﻿using System;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        public static string UserPrefixe = Prefixe("user");

        //----------------------------------------------------------------------------------------------------------

        void UpdateStdin(in bool ctab, in bool csubmit)
        {
            Event e = Event.current;
            Boa boa = boas[^1];
            bool uparrow = e.type == EventType.KeyDown && e.keyCode == KeyCode.UpArrow;
            bool downarrow = e.type == EventType.KeyDown && e.keyCode == KeyCode.DownArrow;

            if (uparrow || downarrow)
            {
                if (GetHistory(uparrow ? -1 : 1, out string line))
                {
                    stdin.text = line;
                    RequestCursorMove(line.Length, true);
                }
                e.Use();
            }
            else if (csubmit || ctab)
                if (csubmit && string.IsNullOrWhiteSpace(stdin.text))
                {
                    stdin.text = string.Empty;
                    if (boa.HasFlags(Boa.FlagsF.Closable))
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

                    LineParser line = new(this, csubmit ? stdin.text : stdinOld, cmdM, stdinOld.Length);

                    try
                    {
                        string temp = stdin.text;
                        if (csubmit)
                        {
                            string log = boa.prefixe + stdin.text;
                            if (this == terminal)
                                print(log);
                            else
                                AddLine(log);
                            stdin.text = string.Empty;
                        }

                        boa.OnCmdLine(line);

                        if (csubmit && boa == boas[0])
                            AddToHistory(temp);

                        if (ctab || line.isCpl)
                        {
                            stdin.text = line.rawtext;
                            RequestCursorMove(line.sel_move, false);
                        }
                    }
                    catch (WrongCommandException ex)
                    {
                        Debug.LogWarning($"{GetType()} -> unknown command: \"{ex.Message.Bold()}\"");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
        }
    }
}