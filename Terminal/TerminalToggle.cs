using UnityEngine;

namespace _TERMINAL_
{
    internal class TerminalToggle : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyUp(KeyCode.P))
                if (!Terminal.instance.enabled)
                    Terminal.instance.ToggleWindow(true);
        }
    }
}