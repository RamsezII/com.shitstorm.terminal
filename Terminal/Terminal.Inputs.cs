using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        void UpdateInputs()
        {
            if (Input.GetKeyDown(KeyCode.T))
                ToggleWindow(true);
        }

        static (bool ctab, bool csubmit) CatchTabAndEnter(bool focus)
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                bool ctab = e.character == '\t';
                bool csubmit = e.character == '\n' || e.character == '\r';

                if (focus)
                {
                    bool ktab = e.keyCode == KeyCode.Tab;
                    bool ksubmit = e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter;

                    if (ktab || ksubmit || ctab || csubmit)
                        e.Use();

#if PLATFORM_STANDALONE_LINUX
                    ctab |= ktab;
#endif
                }

                return (ctab, csubmit);
            }
            else
                return default;
        }
    }
}