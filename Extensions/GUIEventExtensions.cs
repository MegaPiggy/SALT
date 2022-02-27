using UnityEngine;

/// <summary>Contains extension methods for Events (from the Legacy GUI system)</summary>
internal static class GUIEventExtensions
{
    private static Event lastEvent;
    private static EventType lastType = EventType.Ignore;

    /// <summary>
    /// Ignores an event if the type matches and (in case provided) the condition also matches
    /// </summary>
    /// <param name="this">This (gui) event</param>
    /// <param name="typeToIgnore">The type to ignore if found</param>
    /// <param name="condition">The additional condition to check</param>
    /// <returns>True if ignored, false otherwise</returns>
    public static bool IgnoreIf(this Event @this, EventType typeToIgnore, bool condition = true)
    {
        if (!condition || @this.type != typeToIgnore)
            return false;
        @this.type = EventType.Ignore;
        lastEvent = @this;
        lastType = typeToIgnore;
        return true;
    }

    /// <summary>Restores the event to it's normal state</summary>
    /// <param name="this">This (gui) event</param>
    public static void Restore(this Event @this)
    {
        if (!@this.Equals((object)lastEvent) || lastType == EventType.Ignore)
            return;
        @this.type = lastType;
        lastType = EventType.Ignore;
        lastEvent = (Event)null;
    }
}