using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ObjectiveUnityEvent : UnityEvent<ObjectiveData> { }

public class ObjectiveEventListener : MonoBehaviour
{
    [Tooltip("The event to listen for.")]
    public ObjectiveEvent Event;

    [Tooltip("The action to execute when the event is raised.")]
    public ObjectiveUnityEvent Response;

    private void OnEnable()
    {
        Event?.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event?.UnregisterListener(this);
    }

    public void OnEventRaised(ObjectiveData data)
    {
        Response.Invoke(data);
    }
} 