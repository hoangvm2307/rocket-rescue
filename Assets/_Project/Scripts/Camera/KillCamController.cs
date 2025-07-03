// File: KillCamController.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KillCamController : MonoBehaviour
{
    [Header("Picture-in-Picture Setup")]
    [Tooltip("Kéo GameObject của camera PiP vào đây")]
    [SerializeField] private Camera pipCamera;
    
    [Tooltip("Kéo Image của khung viền UI vào đây")]
    [SerializeField] private Image pipBorder;

    [Header("Effects")]
    [Tooltip("Thời gian kill cam hiển thị trên màn hình")]
    [SerializeField] private float displayDuration = 2.0f;

    [Tooltip("Tốc độ game khi slow-motion")]
    [Range(0.01f, 1f)]
    [SerializeField] private float slowMotionScale = 0.1f;

    [Header("2.5D Camera Settings")] // <-- THÊM CÀI ĐẶT MỚI
    [Tooltip("Khoảng cách của camera PiP trên trục Z so với enemy")]
    [SerializeField] private float cameraDistance = 15f;
    [Tooltip("Độ cao của camera PiP so với enemy")]
    [SerializeField] private float cameraHeightOffset = 1.5f;

    private Coroutine focusCoroutine;
    private float originalFixedDeltaTime;

    void Start()
    {
        if (pipCamera != null) pipCamera.gameObject.SetActive(false);
        if (pipBorder != null) pipBorder.gameObject.SetActive(false);
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void StartFocusSequence(GameObject enemyObject)
    {
        if (pipCamera == null || enemyObject == null) return;
        if (focusCoroutine != null) return; 
        focusCoroutine = StartCoroutine(DoPiPSequence(enemyObject.transform));
    }

    private IEnumerator DoPiPSequence(Transform target)
    {
        // --- Phần Slow-motion và bật camera/border giữ nguyên ---
        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;
        pipCamera.gameObject.SetActive(true);
        if (pipBorder != null) pipBorder.gameObject.SetActive(true);

        // --- PHẦN LOGIC ĐỊNH VỊ CAMERA ĐÃ ĐƯỢC THAY ĐỔI ---
        // 1. Lấy vị trí của mục tiêu
        Vector3 targetPosition = target.position;

        // 2. Tạo vị trí cho camera bằng cách offset trên trục Z
        Vector3 cameraPosition = new Vector3(
            targetPosition.x,                           // Giữ nguyên trục X của mục tiêu
            targetPosition.y + cameraHeightOffset,      // Đặt camera cao hơn mục tiêu một chút
            -cameraDistance                             // Luôn đặt camera ở một khoảng cách cố định trên trục Z
        );

        // 3. Cập nhật vị trí và hướng nhìn của camera
        pipCamera.transform.position = cameraPosition;
        pipCamera.transform.LookAt(targetPosition + (Vector3.up * cameraHeightOffset));
        // ----------------------------------------------------

        // --- Phần đợi và tắt hiệu ứng giữ nguyên ---
        yield return new WaitForSecondsRealtime(displayDuration);
        pipCamera.gameObject.SetActive(false);
        if (pipBorder != null) pipBorder.gameObject.SetActive(false);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;
        focusCoroutine = null;
    }
}