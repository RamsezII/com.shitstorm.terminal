using _ARK_;
using UnityEngine;

namespace _TERMINAL_
{
    internal class TerminalToggle : MonoBehaviour
    {
        private void OnEnable()
        {
            NUCLEOR.delegates.onInputs -= UpdateInputs;
            NUCLEOR.delegates.onInputs += UpdateInputs;
        }

        private void OnDisable()
        {
            NUCLEOR.delegates.onInputs -= UpdateInputs;
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