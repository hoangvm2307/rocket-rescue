using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectiveData
{
    public int enemiesRemaining;
    public int hostagesRemaining;

    public ObjectiveData(int enemies, int hostages)
    {
        enemiesRemaining = enemies;
        hostagesRemaining = hostages;
    }
}

[CreateAssetMenu(menuName = "Game Event/Objective Event", fileName = "New Objective Event")]
public class ObjectiveEvent : ScriptableObject
{
    private readonly List<ObjectiveEventListener> listeners = new List<ObjectiveEventListener>();

    public void Raise(ObjectiveData data)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(data);
        }
    }

    public void RegisterListener(ObjectiveEventListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnregisterListener(ObjectiveEventListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }
} 