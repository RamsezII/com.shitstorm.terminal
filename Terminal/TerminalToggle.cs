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
                    if (Application.isEditor
                        || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)
                        || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)
                        || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)
                        || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))
                        Terminal.terminal.ToggleWindow(true);
        }
    }
}