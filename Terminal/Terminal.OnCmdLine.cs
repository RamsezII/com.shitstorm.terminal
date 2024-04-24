using System;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        const string
            str_chainCmds = "&&",
            str_pipe2muon = "|",
            str_pipe2file = ">";

        //----------------------------------------------------------------------------------------------------------

        public virtual void OnCmdLine(in LineParser line)
        {
            do
            {
                try
                {
                    boas[^1].OnCmdLine(line);
                    if (line.TryReadEnd(out string read))
                        switch (read)
                        {
                            case str_pipe2muon:
                            case str_pipe2file:
                                Debug.LogWarning($"dead pipe \"{read}\"");
                                return;

                            case str_chainCmds:
                                break;

                            default:
                                Debug.LogWarning($"what was \"{read}\" meant for?");
                                $"use \"&&\" to chain commands".LogAsTip();
                                return;
                        }
                    else
                        break;
                }
                catch (WrongCommandException e)
                {
                    Debug.LogWarning($"{nameof(WrongCommandException)}: \"{e.Message.Bold()}\"");
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return;
                }
            }
            while (line.Until(null, " "));
        }
    }
}