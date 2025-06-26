using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntUnityEvent : UnityEvent<int> { }

public class IntEventListener : MonoBehaviour
{
    [Tooltip("The event to listen for.")]
    public IntEvent Event;

    [Tooltip("The action to execute when the event is raised.")]
    public IntUnityEvent Response;

    private void OnEnable()
    {
        Event?.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event?.UnregisterListener(this);
    }

    public void OnEventRaised(int value)
    {
        Response.Invoke(value);
    }
} 