using _ARK_;
using UnityEngine;

namespace _TERMINAL_
{
    internal class TerminalToggle : MonoBehaviour
    {
        private void OnEnable()
        {
            NUCLEOR.delegates.getInputs -= UpdateInputs;
            NUCLEOR.delegates.getInputs += UpdateInputs;
        }

        private void OnDisable()
        {
            NUCLEOR.delegates.getInputs -= UpdateInputs;
        }

        void UpdateInputs()
        {
            if (!NUCLEOR.inputsUsers.isUsed._value)
                if (!Terminal.instance.Enabled)
                    if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyUp(KeyCode.P))
                        Terminal.instance.ToggleWindow(true);
        }
    }
}