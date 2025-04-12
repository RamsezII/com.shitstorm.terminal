using System.Linq;
using System;
using UnityEngine;

namespace _TERMINAL_
{
    partial class CmdGameObjects
    {
        void CmdFindRoot(in LineParser line)
        {
            string goPath = line.Read();
            if (line.IsCplThis)
                line.OnCpls(goPath, ERootGameObjects_quotes());
            else if (TryFindRootGameObjectByName(goPath, out GameObject go))
                if (line.TryRead(out string arg1))
                {
                    if (line.IsCplThis)
                        line.OnCpls(arg1, Enumerable.Range(0, (int)SubCodes._last_).Select(i => ((SubCodes)i).ToString()));
                    else if (Enum.TryParse(arg1, true, out SubCodes subcode) && subcode < SubCodes._last_)
                    {
                        if (line.IsExec)
                            switch (subcode)
                            {
                                case SubCodes.Enable:
                                    go.SetActive(true);
                                    break;

                                case SubCodes.Disable:
                                    go.SetActive(false);
                                    break;

                                default:
                                    Debug.LogWarning($"Unimplemented command: \"{subcode}\" ({this})");
                                    break;
                            }
                    }
                }
                else
                {
                    if (line.IsExec)
                        Debug.Log(go.activeSelf);
                }
            else
                Debug.LogWarning($"Could not find gameobject: \"{goPath}\" ({this})");
        }
    }
}