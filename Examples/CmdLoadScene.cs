using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _TERMINAL_
{
    internal class CmdScenes : IShell
    {
        enum Codes
        {
            LoadSceneByBuildName,
            ListScenesInBuild,
            _last_,
        }

        static string[] scenes = new string[SceneManager.sceneCountInBuildSettings];

        IEnumerable<string> IShell.ECommands => Enumerable.Range(0, (int)Codes._last_).Select(i => ((Codes)i).ToString());

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            scenes = new string[SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < scenes.Length; i++)
                scenes[i] = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            Shell.AddUser(new CmdScenes());
        }

        //--------------------------------------------------------------------------------------------------------------

        void IShell.OnCmdLine(in string arg0, in LineParser line)
        {
            if (Enum.TryParse(arg0, true, out Codes code) && code < Codes._last_)
                switch (code)
                {
                    case Codes.LoadSceneByBuildName:
                        {
                            string sceneName = line.Read();
                            if (line.IsCplThis)
                                line.OnCpls(sceneName, scenes);
                            else if (line.IsExec)
                                if (scenes.Contains(sceneName))
                                    SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                                else
                                    Debug.LogError($"Scene \"{sceneName}\" not found in build");
                        }
                        break;

                    case Codes.ListScenesInBuild:
                        if (line.IsExec)
                        {
                            StringBuilder log = new();
                            foreach (var scene in scenes)
                                log.AppendLine(scene);
                            Debug.Log(log);
                        }
                        break;
                }
        }
    }
}