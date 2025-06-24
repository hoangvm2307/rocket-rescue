using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform launchPoint; // Điểm tên lửa sẽ bay ra
    [SerializeField] private GameObject rocketPrefab; // Prefab của tên lửa
    [SerializeField] private LineRenderer lineRenderer; // Component vẽ đường đạn

    [Header("Launch Settings")]
    [SerializeField] private float launchForceMultiplier = 1.5f; // Hệ số nhân lực bắn

    private Camera mainCamera;
    private Plane gameplayPlane; // Mặt phẳng ảo để tính toán input
    private Vector3 dragStartPoint;

    void Awake()
    {
        mainCamera = Camera.main;
        // Tạo một mặt phẳng ảo tại vị trí Z = 0 để bắt input
        gameplayPlane = new Plane(Vector3.forward, Vector3.zero);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Bắt đầu kéo
            dragStartPoint = GetWorldPointFromMouse();
            lineRenderer.enabled = true;
        }
        else if (Input.GetMouseButton(0))
        {
            // Đang kéo, cập nhật đường đạn
            Vector3 dragCurrentPoint = GetWorldPointFromMouse();
            Vector3 launchVector = dragStartPoint - dragCurrentPoint;

            lineRenderer.SetPosition(0, launchPoint.position);
            lineRenderer.SetPosition(1, launchPoint.position + launchVector);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Thả tay, bắn
            LaunchRocket();
            lineRenderer.enabled = false;
        }
    }

    private void LaunchRocket()
    {
        Vector3 dragCurrentPoint = GetWorldPointFromMouse();
        Vector3 launchVector = (dragStartPoint - dragCurrentPoint) * launchForceMultiplier;

        // Giới hạn lực bắn tối đa để tránh bug
        launchVector = Vector3.ClampMagnitude(launchVector, 25f); 

        GameObject rocketInstance = Instantiate(rocketPrefab, launchPoint.position, launchPoint.rotation);
        Rigidbody rocketRb = rocketInstance.GetComponent<Rigidbody>();
        rocketRb.AddForce(launchVector, ForceMode.Impulse);
    }

    // Hàm tiện ích để lấy vị trí trên mặt phẳng 3D từ vị trí chuột 2D
    private Vector3 GetWorldPointFromMouse()
    {
        Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (gameplayPlane.Raycast(mouseRay, out float distance))
        {
            return mouseRay.GetPoint(distance);
        }
        return Vector3.zero;
    }
}