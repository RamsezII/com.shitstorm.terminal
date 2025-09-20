using _ARK_;
using System;
using System.IO;
using UnityEngine;

namespace _TERMINAL_
{
    internal static class CmdSettings
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Shell.root_commands.AddCommand(new(null, "OpenTerminal", onCmd_exe: () => Terminal.instance.ToggleWindow(true)));

            Shell.root_commands.AddCommand(new(null, "CloseTerminal", onCmd_exe: () => Terminal.instance.ToggleWindow(false)));

            Shell.root_commands.AddCommand(new(null, "Shutdown", onCmd_exe: () => Application.Quit()));

            Shell.root_commands.AddCommand(new(null, "ToggleTerminalFullScreen", onCmd_exe: () =>
            {
                Terminal.instance.fullscreen = !Terminal.instance.fullscreen;
                Debug.Log($"{typeof(Terminal).FullName}.{nameof(Terminal.instance.fullscreen)}: {Terminal.instance.fullscreen}");
            }));

            Shell.root_commands.AddCommand(new(null, "Edit", onCmd_line: line =>
            {
                string path = line.ReadAsPath();
                if (line.IsExec)
                    try
                    {
                        FileInfo file = new(path);
                        if (!File.Exists(file.FullName))
                        {
                            string folderPath = Path.GetDirectoryName(file.FullName);
                            if (!Directory.Exists(folderPath))
                                Directory.CreateDirectory(folderPath);
                            Debug.Log($"Creating file: \"{file.FullName}\"".ToSubLog());
                            File.WriteAllText(file.FullName, string.Empty);
                        }
                        Debug.Log($"Opening file: \"{file.FullName}\"".ToSubLog());
                        Application.OpenURL(file.FullName);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e.Message);
                    }
            }));

            Shell.root_commands.AddCommand(new(null, "machine_name", onCmd_line: line =>
            {
                string netName = line.Read();
                if (line.IsExec)
                {
                    if (string.IsNullOrWhiteSpace(netName))
                        Debug.Log(ArkMachine.user_name.Value);
                    else
                        ArkMachine.user_name.Value = netName;
                }
            }));

            Shell.root_commands.AddCommand(new(null, "OpenPlayerLogs", onCmd_exe: () =>
            {
                string logPath = Path.Combine(Application.persistentDataPath, "Player.log");
                if (File.Exists(logPath))
                    Application.OpenURL(logPath);
                else
                    Debug.LogWarning($"Player.log not found at: {logPath}");
            }));

            Shell.root_commands.AddCommand(new(null, "OpenApplicationPersistentDataPath", onCmd_exe: () =>
            {
                string persistentDataPath = Application.persistentDataPath;
                if (Directory.Exists(persistentDataPath))
                    Application.OpenURL(persistentDataPath);
                else
                    Debug.LogWarning($"Path not found: {persistentDataPath}");
            }));
        }
    }
}