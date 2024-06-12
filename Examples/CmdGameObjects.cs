using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _TERMINAL_
{
    internal class CmdGameObjects : Command, Shell.IUser
    {
        enum Codes : byte
        {
            FindRootGameObject,
            ListAllRootObjects,
            _last_,
        }

        enum SubCodes : byte
        {
            Enable,
            Disable,
            IsEnabled,
            _last_,
        }

        IEnumerable<string> Shell.IUser.ECommands => Enumerable.Range(0, (int)Codes._last_).Select(i => ((Codes)i).ToString());

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            Shell.AddUser(new CmdGameObjects());
        }

        static IEnumerable<string> ERootGameObjects_quotes() => ERootGameObjects().Select(go => $"\"{go.name}\"");
        static IEnumerable<GameObject> ERootGameObjects()
        {
            foreach (var go in UnityEngine.Object.FindObjectsOfType<GameObject>(true))
                if (go.transform.parent == null)
                    yield return go;
        }

        static bool TryFindRootGameObjectByName(string path, out GameObject go)
        {
            path = path.Trim('\"');
            foreach (var root in ERootGameObjects())
                if (root.name.Equals(path, StringComparison.OrdinalIgnoreCase))
                {
                    go = root;
                    return true;
                }
            go = null;
            return false;
        }

        void Shell.IUser.OnCmdLine(in string arg0, in LineParser line)
        {
            if (Enum.TryParse(arg0, true, out Codes code) && code < Codes._last_)
                switch (code)
                {
                    case Codes.FindRootGameObject:
                        {
                            string goPath = line.Read();
                            if (line.IsCplThis)
                                line.OnCpls(goPath, ERootGameObjects_quotes());
                            else if (TryFindRootGameObjectByName(goPath, out GameObject go))
                            {
                                string arg1 = line.Read();
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

                                            case SubCodes.IsEnabled:
                                                Debug.Log(go.activeSelf);
                                                break;

                                            default:
                                                Debug.LogWarning($"Unimplemented command: \"{subcode}\" ({this})");
                                                break;
                                        }
                                }
                            }
                            else
                                Debug.LogWarning($"Could not find gameobject: \"{goPath}\" ({this})");
                        }
                        break;

                    case Codes.ListAllRootObjects:
                        if (line.IsExec)
                            foreach (var go in ERootGameObjects())
                                Debug.Log(go.name);
                        break;

                    default:
                        Debug.LogWarning($"Unimplemented command: \"{code}\" ({this})");
                        break;
                }
            else
                base.OnCmdLine(arg0, line);
        }
    }
}