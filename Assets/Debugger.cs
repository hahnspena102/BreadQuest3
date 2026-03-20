using UnityEngine;
using System.Diagnostics;

[System.Flags]
public enum DebugType
{
    None    = 0,
    General = 1 << 0,
    World   = 1 << 1,
    AI      = 1 << 2,
    Combat  = 1 << 3,
    Items   = 1 << 4,
    Enemies = 1 << 5,
    All     = ~0
}

public static class Debugger
{
    // Toggle what debug categories are enabled
    public static DebugType EnabledTypes = DebugType.General | DebugType.World | DebugType.AI | DebugType.Combat | DebugType.Items | DebugType.Enemies;
    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void Log(string message, Object context = null, DebugType type = DebugType.General)
    {
        if ((EnabledTypes & type) != 0)
        {
            UnityEngine.Debug.Log($"[{type}] {message}", context);
        }
    }

    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void LogWarning(string message, Object context = null, DebugType type = DebugType.General)
    {
        if ((EnabledTypes & type) != 0)
        {
            UnityEngine.Debug.LogWarning($"[{type}] {message}", context);
        }
    }

    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void LogError(string message, Object context = null, DebugType type = DebugType.General)
    {
        if ((EnabledTypes & type) != 0)
        {
            UnityEngine.Debug.LogError($"[{type}] {message}", context);
        }
    }
}