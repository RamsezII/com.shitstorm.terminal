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
            Enable,
            Disable,
            _last_,
        }

        IEnumerable<string> Shell.IUser.ECommands { get { yield return nameof(GameObject); } }

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
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
            if (arg0.Equals(nameof(GameObject), StringComparison.OrdinalIgnoreCase))
            {
                string goPath = line.Read();
                if (line.IsCplThis)
                    line.OnCpls(goPath, ERootGameObjects_quotes());
                else if (TryFindRootGameObjectByName(goPath, out GameObject go))
                {
                    string arg1 = line.Read();
                    if (line.IsCplThis)
                        line.OnCpls(arg1, Enumerable.Range(0, (int)Codes._last_).Select(i => ((Codes)i).ToString()));
                    else if (Enum.TryParse(arg1, true, out Codes code) && code < Codes._last_)
                    {
                        if (line.IsExec)
                            switch (code)
                            {
                                case Codes.Enable:
                                    go.SetActive(true);
                                    break;

                                case Codes.Disable:
                                    go.SetActive(false);
                                    break;

                                default:
                                    Debug.LogWarning($"Unimplemented command: \"{code}\" ({this})");
                                    break;
                            }
                    }
                }
                else
                {
                    Debug.LogWarning($"Could not find gameobject: \"{goPath}\" ({this})");
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------

        public CmdGameObjects()
        {

        }
    }
}