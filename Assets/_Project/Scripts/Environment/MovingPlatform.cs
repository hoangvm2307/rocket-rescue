// File: Scripts/Environment/MovingPlatform.cs
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    
    // Thuộc tính công khai để các đối tượng khác (như Player) có thể đọc được vận tốc
    public Vector3 Velocity { get; private set; }

    private int currentWaypointIndex = 0;
    private Vector3 lastPosition;

    void Start()
    {
        // Khởi tạo vị trí ban đầu
        lastPosition = transform.position;
    }

    // Nên dùng FixedUpdate cho các thao tác vật lý để đồng bộ với Rigidbody của Player
    void FixedUpdate()
    {
        // 1. DI CHUYỂN PLATFORM (Logic này giữ nguyên)
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        // Sử dụng rb.MovePosition để di chuyển vật lý mượt hơn
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.fixedDeltaTime);
        transform.position = newPosition;

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        // 2. TÍNH TOÁN VẬN TỐC
        // Vận tốc = (Vị trí mới - Vị trí cũ) / Thời gian
        Velocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = transform.position;
    }

    // 3. XÓA BỎ HOÀN TOÀN OnCollisionEnter VÀ OnCollisionExit
    // Không còn sử dụng SetParent nữa.
}