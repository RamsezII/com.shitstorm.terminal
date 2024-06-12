using UnityEngine;

namespace _TERMINAL_
{
    internal class TerminalToggle : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
                if (!Terminal.terminal.enabled)
                    Terminal.terminal.ToggleWindow(true);
        }
    }
}