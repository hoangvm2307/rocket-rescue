// File: PlayerInput.cs
using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    // Sử dụng UnityEvent để các script khác có thể lắng nghe trực tiếp từ Inspector
    public UnityEvent<Vector3> OnDragStart;
    public UnityEvent<Vector3> OnDrag;
    public UnityEvent<Vector3> OnDragEnd;

    private Camera mainCamera;
    private float dragPlaneZ;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Gửi đi tọa độ màn hình thay vì tọa độ thế giới
            OnDragStart.Invoke(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            // Gửi đi tọa độ màn hình thay vì tọa độ thế giới
            OnDrag.Invoke(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Gửi đi tọa độ màn hình thay vì tọa độ thế giới
            OnDragEnd.Invoke(Input.mousePosition);
        }
    }
}