using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Event/Int Event", fileName = "New Int Event")]
public class IntEvent : ScriptableObject
{
    private readonly List<IntEventListener> listeners = new List<IntEventListener>();

    public void Raise(int value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(IntEventListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnregisterListener(IntEventListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }
} 