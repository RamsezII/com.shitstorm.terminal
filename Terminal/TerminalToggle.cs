using UnityEngine;
using UnityEngine.InputSystem;

namespace _TERMINAL_
{
    internal class TerminalToggle : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current.pKey.wasPressedThisFrame)
                if (!Terminal.terminal.enabled)
                    Terminal.terminal.ToggleWindow(true);
        }
    }
}