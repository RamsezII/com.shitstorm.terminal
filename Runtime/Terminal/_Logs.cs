using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        static void OnLogMessageReceived(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Warning && message.StartsWith("The character with Unicode value "))
                return;

            message = message.TrimEnd('\n', '\r');
            switch (type)
            {
                case LogType.Error:
                    AddLine($"<color=\"orange\">{message}</color>");
                    break;
                case LogType.Assert:
                    AddLine($"<color=\"red\">{message.Bold()}</color>\n<color=#BB2222>{stackTrace.TrimEnd('\n', '\r')}</color>");
                    break;
                case LogType.Warning:
                    AddLine($"<color=\"yellow\">{message}</color>");
                    break;
                case LogType.Exception:
                    AddLine($"<color=\"red\">{message.Bold()}</color>\n<color=#BB2222>{stackTrace.TrimEnd('\n', '\r')}</color>");
                    break;
                default:
                    AddLine(message);
                    break;
            }
        }
    }
}