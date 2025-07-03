using UnityEngine;

// File: ScreenShake.cs (Có thể đặt trong Scripts/Core/ hoặc Scripts/Camera/)
public class ScreenShake : MonoBehaviour
{
    private Vector3 originalPos;
    private float timeRemaining;
    private float magnitude;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void LateUpdate()
    {
        if (timeRemaining > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * magnitude;
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = originalPos;
        }
    }
    public void TriggerShake(float duration, float magnitude)
    {
        this.timeRemaining = duration;
        this.magnitude = magnitude;
    }
}
