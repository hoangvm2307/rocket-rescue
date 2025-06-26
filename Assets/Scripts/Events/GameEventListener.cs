// File: GameEventListener.cs
using UnityEngine;
using UnityEngine.Events; // Cần dùng UnityEvent

public class GameEventListener : MonoBehaviour
{
    [Tooltip("Sự kiện cần lắng nghe")]
    public GameEvent Event;

    [Tooltip("Hành động sẽ thực thi khi sự kiện được bắn ra")]
    public UnityEvent Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        Response.Invoke();
    }
}