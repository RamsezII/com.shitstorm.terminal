using _ARK_;
using UnityEngine;

namespace _TERMINAL_
{
    internal class TerminalToggle : MonoBehaviour
    {
        private void OnEnable()
        {
            NUCLEOR.onInputs -= UpdateInputs;
            NUCLEOR.onInputs += UpdateInputs;
        }

        private void OnDisable()
        {
            NUCLEOR.onInputs -= UpdateInputs;
        }

        void UpdateInputs()
        {
            if (!Terminal.instance.Enabled && (CommandLineUI.instance == null || !CommandLineUI.instance.Enabled))
            {
                if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyUp(KeyCode.P))
                    Terminal.instance.ToggleWindow(true);
                else if (Input.GetKeyDown(KeyCode.O) || Input.GetKeyUp(KeyCode.O))
                    CommandLineUI.instance.Toggle();
            }
        }
    }
}