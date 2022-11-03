using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void Callback();
public delegate void Callback<T>(T arg1);
public delegate void Callback<T, U>(T arg1, U arg2);
public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);
public delegate void Callback<T, U, V, W>(T arg1, U arg2, V arg3, W arg4);

public static class GlobalMessenger
{
    private static IDictionary<string, EventData> eventTable = new Dictionary<string, EventData>(ComparerLibrary.stringEqComparer);

    public static void AddListener(string eventType, Callback handler)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
            {
                eventData = new EventData();
                eventTable.Add(eventType, eventData);
            }
            eventData.callbacks.Add(handler);
        }
    }

    public static void RemoveListener(string eventType, Callback handler)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
                return;
            int index = eventData.callbacks.IndexOf(handler);
            if (index < 0)
                return;
            eventData.callbacks[index] = eventData.callbacks[eventData.callbacks.Count - 1];
            eventData.callbacks.RemoveAt(eventData.callbacks.Count - 1);
        }
    }

    public static void FireEvent(string eventType)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
                return;
            eventData.isInvoking = !eventData.isInvoking ? true : throw new InvalidOperationException("GlobalMessenger does not support recursive FireEvent calls to the same eventType.");
            eventData.temp.AddRange(eventData.callbacks);
            for (int index = 0; index < eventData.temp.Count; ++index)
            {
                try
                {
                    eventData.temp[index]();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            eventData.temp.Clear();
            eventData.isInvoking = false;
        }
    }

    private class EventData
    {
        public List<Callback> callbacks = new List<Callback>();
        public List<Callback> temp = new List<Callback>();
        public bool isInvoking;
    }
}


public static class GlobalMessenger<T>
{
    private static IDictionary<string, EventData> eventTable = new Dictionary<string, EventData>(ComparerLibrary.stringEqComparer);

    public static void AddListener(string eventType, Callback<T> handler)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
            {
                eventData = new EventData();
                eventTable.Add(eventType, eventData);
            }
            eventData.callbacks.Add(handler);
        }
    }

    public static void RemoveListener(string eventType, Callback<T> handler)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
                return;
            int index = eventData.callbacks.IndexOf(handler);
            if (index < 0)
                return;
            eventData.callbacks[index] = eventData.callbacks[eventData.callbacks.Count - 1];
            eventData.callbacks.RemoveAt(eventData.callbacks.Count - 1);
        }
    }

    public static void FireEvent(string eventType, T arg1)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
                return;
            eventData.isInvoking = !eventData.isInvoking ? true : throw new InvalidOperationException("GlobalMessenger does not support recursive FireEvent calls to the same eventType.");
            eventData.temp.AddRange(eventData.callbacks);
            for (int index = 0; index < eventData.temp.Count; ++index)
            {
                try
                {
                    eventData.temp[index](arg1);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            eventData.temp.Clear();
            eventData.isInvoking = false;
        }
    }

    private class EventData
    {
        public List<Callback<T>> callbacks = new List<Callback<T>>();
        public List<Callback<T>> temp = new List<Callback<T>>();
        public bool isInvoking;
    }
}

public static class GlobalMessenger<T, U>
{
    private static IDictionary<string, EventData> eventTable = new Dictionary<string, EventData>(ComparerLibrary.stringEqComparer);

    public static void AddListener(string eventType, Callback<T, U> handler)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
            {
                eventData = new EventData();
                eventTable.Add(eventType, eventData);
            }
            eventData.callbacks.Add(handler);
        }
    }

    public static void RemoveListener(string eventType, Callback<T, U> handler)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
                return;
            int index = eventData.callbacks.IndexOf(handler);
            if (index < 0)
                return;
            eventData.callbacks[index] = eventData.callbacks[eventData.callbacks.Count - 1];
            eventData.callbacks.RemoveAt(eventData.callbacks.Count - 1);
        }
    }

    public static void FireEvent(string eventType, T arg1, U arg2)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
                return;
            eventData.isInvoking = !eventData.isInvoking ? true : throw new InvalidOperationException("GlobalMessenger does not support recursive FireEvent calls to the same eventType.");
            eventData.temp.AddRange(eventData.callbacks);
            for (int index = 0; index < eventData.temp.Count; ++index)
            {
                try
                {
                    eventData.temp[index](arg1, arg2);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            eventData.temp.Clear();
            eventData.isInvoking = false;
        }
    }

    private class EventData
    {
        public List<Callback<T, U>> callbacks = new List<Callback<T, U>>();
        public List<Callback<T, U>> temp = new List<Callback<T, U>>();
        public bool isInvoking;
    }
}

public static class GlobalMessenger<T, U, V>
{
    private static IDictionary<string, EventData> eventTable = new Dictionary<string, EventData>(ComparerLibrary.stringEqComparer);

    public static void AddListener(string eventType, Callback<T, U, V> handler)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
            {
                eventData = new EventData();
                eventTable.Add(eventType, eventData);
            }
            eventData.callbacks.Add(handler);
        }
    }

    public static void RemoveListener(string eventType, Callback<T, U, V> handler)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
                return;
            int index = eventData.callbacks.IndexOf(handler);
            if (index < 0)
                return;
            eventData.callbacks[index] = eventData.callbacks[eventData.callbacks.Count - 1];
            eventData.callbacks.RemoveAt(eventData.callbacks.Count - 1);
        }
    }

    public static void FireEvent(string eventType, T arg1, U arg2, V arg3)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
                return;
            eventData.isInvoking = !eventData.isInvoking ? true : throw new InvalidOperationException("GlobalMessenger does not support recursive FireEvent calls to the same eventType.");
            eventData.temp.AddRange(eventData.callbacks);
            for (int index = 0; index < eventData.temp.Count; ++index)
            {
                try
                {
                    eventData.temp[index](arg1, arg2, arg3);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            eventData.temp.Clear();
            eventData.isInvoking = false;
        }
    }

    private class EventData
    {
        public List<Callback<T, U, V>> callbacks = new List<Callback<T, U, V>>();
        public List<Callback<T, U, V>> temp = new List<Callback<T, U, V>>();
        public bool isInvoking;
    }
}

public static class GlobalMessenger<T, U, V, W>
{
    private static IDictionary<string, EventData> eventTable = new Dictionary<string, EventData>(ComparerLibrary.stringEqComparer);

    public static void AddListener(string eventType, Callback<T, U, V, W> handler)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
            {
                eventData = new EventData();
                eventTable.Add(eventType, eventData);
            }
            eventData.callbacks.Add(handler);
        }
    }

    public static void RemoveListener(string eventType, Callback<T, U, V, W> handler)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
                return;
            int index = eventData.callbacks.IndexOf(handler);
            if (index < 0)
                return;
            eventData.callbacks[index] = eventData.callbacks[eventData.callbacks.Count - 1];
            eventData.callbacks.RemoveAt(eventData.callbacks.Count - 1);
        }
    }

    public static void FireEvent(string eventType, T arg1, U arg2, V arg3, W arg4)
    {
        lock (eventTable)
        {
            EventData eventData;
            if (!eventTable.TryGetValue(eventType, out eventData))
                return;
            eventData.isInvoking = !eventData.isInvoking ? true : throw new InvalidOperationException("GlobalMessenger does not support recursive FireEvent calls to the same eventType.");
            eventData.temp.AddRange(eventData.callbacks);
            for (int index = 0; index < eventData.temp.Count; ++index)
            {
                try
                {
                    eventData.temp[index](arg1, arg2, arg3, arg4);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            eventData.temp.Clear();
            eventData.isInvoking = false;
        }
    }

    private class EventData
    {
        public List<Callback<T, U, V, W>> callbacks = new List<Callback<T, U, V, W>>();
        public List<Callback<T, U, V, W>> temp = new List<Callback<T, U, V, W>>();
        public bool isInvoking;
    }
}