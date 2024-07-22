using UnityEngine;

namespace _TERMINAL_
{
    internal class TerminalToggle : MonoBehaviour
    {
        private void Update()
        {
            if (Terminal.instance.enabled)
            {
                if (Input.GetKeyDown(KeyCode.F11) || Input.GetKeyUp(KeyCode.F11))
                    Terminal.instance.fullscreen = !Terminal.instance.fullscreen;
            }
            else if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyUp(KeyCode.P))
                Terminal.instance.ToggleWindow(true);
        }
    }
}