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
                NUCLEOR.instance.sequencer_mono.LogStatus();
                NUCLEOR.instance.sequencer_multi.LogStatus();
            }));
        }
    }
}