using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Vector3UnityEvent : UnityEvent<Vector3> { }  

public class Vector3EventListener : MonoBehaviour
{
    [Tooltip("The event to listen for.")]
    public Vector3Event Event;  

    [Tooltip("The action to execute when the event is raised.")]
    public Vector3UnityEvent Response;  

    private void OnEnable()
    {
        Event?.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event?.UnregisterListener(this);
    }

    public void OnEventRaised(Vector3 value)
    {
        Response.Invoke(value);
    }
}