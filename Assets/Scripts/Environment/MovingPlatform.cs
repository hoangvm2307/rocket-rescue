// File: MovingPlatform.cs (Có thể đặt trong Scripts/Environment/)
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    private int currentWaypointIndex = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    // Khi người chơi chạm vào bệ đỡ
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Gán Player làm con của bệ đỡ để di chuyển cùng nhau
            collision.transform.SetParent(transform);
        }
    }

    // Khi người chơi rời khỏi bệ đỡ
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Hủy việc gán làm con để Player có thể di chuyển tự do
            collision.transform.SetParent(null);
        }
    }
}