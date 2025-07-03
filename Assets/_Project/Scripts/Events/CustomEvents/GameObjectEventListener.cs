// File: Scripts/Events/CustomEvents/GameObjectEventListener.cs
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameObjectUnityEvent : UnityEvent<GameObject> { }

public class GameObjectEventListener : MonoBehaviour
{
    [Tooltip("The event to listen for.")]
    public GameObjectEvent Event;

    [Tooltip("The action to execute when the event is raised.")]
    public GameObjectUnityEvent Response;

    private void OnEnable()
    {
        Event?.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event?.UnregisterListener(this);
    }

    public void OnEventRaised(GameObject value)
    {
        Response.Invoke(value);
    }
}