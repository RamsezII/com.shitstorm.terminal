using _ARK_;
using UnityEngine;

namespace _TERMINAL_
{
    static class CmdNucleor
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Shell.root_commands.AddCommand(new(null, typeof(Scheduler) + "Status", onCmd_exe: () =>
            {
                NUCLEOR.instance.scheduler_sequential.LogStatus();
                NUCLEOR.instance.scheduler_parallel.LogStatus();
            }));
        }
    }
}