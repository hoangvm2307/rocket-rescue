using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game Event/Vector3 Event", fileName = "New Vector3 Event")]
public class Vector3Event : ScriptableObject
{
    private readonly List<Vector3EventListener> listeners = new List<Vector3EventListener>();

    public void Raise(Vector3 value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(Vector3EventListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnregisterListener(Vector3EventListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }
}