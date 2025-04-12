using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _TERMINAL_
{
    internal partial class CmdGameObjects : Command, IShell
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
            _last_,
        }

        IEnumerable<string> IShell.ECommands => Enumerable.Range(0, (int)Codes._last_).Select(i => ((Codes)i).ToString());

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

        void IShell.OnCmdLine(in string arg0, in LineParser line)
        {
            if (Enum.TryParse(arg0, true, out Codes code) && code < Codes._last_)
                switch (code)
                {
                    case Codes.FindRootGameObject:
                        CmdFindRoot(line);
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