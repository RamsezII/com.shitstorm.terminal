using _ARK_;
using UnityEngine;

namespace _TERMINAL_
{
    static class CmdNucleor
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Shell.root_commands.AddCommand(new(null, typeof(Sequencer) + "Status", onCmd_exe: () =>
            {
                NUCLEOR.instance.monolith.LogStatus();
                NUCLEOR.instance.routinizer.LogStatus();
            }));
        }
    }
}