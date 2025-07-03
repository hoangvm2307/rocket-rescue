// File: Scripts/Events/CustomEvents/GameObjectEvent.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Event/GameObject Event", fileName = "New GameObject Event")]
public class GameObjectEvent : ScriptableObject
{
    private readonly List<GameObjectEventListener> listeners = new List<GameObjectEventListener>();

    public void Raise(GameObject value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(GameObjectEventListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnregisterListener(GameObjectEventListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }
}