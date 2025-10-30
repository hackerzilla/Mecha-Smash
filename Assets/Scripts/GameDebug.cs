using UnityEngine;

public static class GameDebug
{
    public static bool enabled = true;

    public static void Log(object message)
    {
        if (enabled) 
            Debug.Log(message);
    }

    public static void Warn(object message)
    {
        if (enabled) 
            Debug.LogWarning(message);
    }

    public static void Error(object message)
    {
        if (enabled)
            Debug.LogError(message);
    }
}