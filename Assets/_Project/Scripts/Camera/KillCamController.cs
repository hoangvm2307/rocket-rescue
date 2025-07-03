using UnityEngine;
using UnityEngine.UI; // Thêm thư viện này để điều khiển Image
using System.Collections;

public class KillCamController : MonoBehaviour // Đổi tên class cho phù hợp
{
    [Header("Picture-in-Picture Setup")]
    [Tooltip("Kéo GameObject của camera PiP vào đây")]
    [SerializeField] private Camera pipCamera;
    
    [Tooltip("Kéo Image của khung viền UI vào đây")]
    [SerializeField] private Image pipBorder;

    [Header("Effects")]
    [Tooltip("Thời gian kill cam hiển thị trên màn hình")]
    [SerializeField] private float displayDuration = 2.0f;

    [Tooltip("Tốc độ game khi slow-motion (ví dụ: 0.1 là 10% tốc độ gốc)")]
    [Range(0.01f, 1f)]
    [SerializeField] private float slowMotionScale = 0.1f;

    private Coroutine focusCoroutine;
    private float originalFixedDeltaTime; // Biến để lưu giá trị gốc

    void Start()
    {
        // Đảm bảo camera và border đã tắt khi bắt đầu game
        if (pipCamera != null)
        {
            pipCamera.gameObject.SetActive(false);
        }
        if (pipBorder != null)
        {
            pipBorder.gameObject.SetActive(false);
        }
        
        // Lưu lại giá trị gốc của fixedDeltaTime
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    // Hàm này được gọi bởi GameEventListener khi một enemy bị tiêu diệt
    public void StartFocusSequence(GameObject enemyObject)
    {
        if (pipCamera == null || enemyObject == null) return;

        if (focusCoroutine != null)
        {
            // Nếu một kill cam khác đang chạy, chúng ta không làm gì cả để tránh rối
            // Hoặc bạn có thể StopCoroutine(focusCoroutine) nếu muốn kill cam mới đè lên cái cũ
            return; 
        }

        focusCoroutine = StartCoroutine(DoPiPSequence(enemyObject.transform));
    }

    private IEnumerator DoPiPSequence(Transform target)
    {
        // 1. Bật Slow-motion
        Time.timeScale = slowMotionScale;
        // Cập nhật fixedDeltaTime để vật lý (Rigidbody) chạy đúng trong slow-motion
        Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;

        // 2. Bật camera và khung viền
        pipCamera.gameObject.SetActive(true);
        if (pipBorder != null) pipBorder.gameObject.SetActive(true);

        // 3. Định vị camera (giữ nguyên logic cũ)
        Vector3 cameraPosition = target.position - (target.forward * 5f) + (Vector3.up * 2f);
        pipCamera.transform.position = cameraPosition;
        pipCamera.transform.LookAt(target.position + Vector3.up * 1f);

        // 4. Đợi hết thời gian hiển thị
        // QUAN TRỌNG: Phải dùng WaitForSecondsRealtime vì Time.timeScale đã bị thay đổi
        yield return new WaitForSecondsRealtime(displayDuration);

        // 5. Tắt camera và khung viền
        pipCamera.gameObject.SetActive(false);
        if (pipBorder != null) pipBorder.gameObject.SetActive(false);

        // 6. Trả lại tốc độ bình thường cho game
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime; // Khôi phục lại giá trị gốc

        focusCoroutine = null;
    }
}