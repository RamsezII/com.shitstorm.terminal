using UnityEngine;

public static partial class Util_terminal
{
    public static char GetRotator(in float speed = 10) => ((int)(Time.unscaledTime * speed) % 4) switch
    {
        0 => '|',
        1 => '/',
        2 => '-',
        3 => '\\',
        _ => '?',
    };
}