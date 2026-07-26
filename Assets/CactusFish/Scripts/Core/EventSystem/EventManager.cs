using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    private static readonly Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();

    private static readonly object _lock = new object();

    public static void Subscribe<T>(Action<T> action) where T : IEvent
    {
        lock (_lock)
        {
            Type type = typeof(T);
            if (!_events.ContainsKey(type))
            {
                _events[type] = action;
            }
            else
            {
                _events[type] = Delegate.Combine(_events[type], action);
            }
        }
    }

    public static void Unscribe<T>(Action<T> action) where T : IEvent
    {
        lock (_lock)
        {
            Type type = typeof(T);
            if (!_events.TryGetValue(type, out var del))
            {
                return;
            }

            var newDel = Delegate.Remove(del, action);
            if (newDel != null)
            {
                _events[type] = newDel;
            }
            else
            {
                _events.Remove(type);
            }
        }
    }

    public static void Publish<T>(T evt) where T : IEvent
    {
        Type type = typeof(T);
        lock (_lock)
        {
            if (!_events.TryGetValue(type, out var del)) return;

            var handlers = (Action<T>)del;

            foreach (var handler in handlers.GetInvocationList())
            {
                try
                {
                    ((Action<T>)handler)(evt);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                }
            }
        }
    }

    public static void ClearAll()
    {
        lock (_lock)
        {
            _events.Clear();
        }
    }
}

