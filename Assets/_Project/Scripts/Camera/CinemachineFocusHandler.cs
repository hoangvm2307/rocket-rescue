using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CinemachineFocusHandler : MonoBehaviour
{
    [Header("Cinemachine References")]
    [SerializeField] private CinemachineCamera playerVCam; // Kéo thả Player Virtual Camera vào đây
    [SerializeField] private CinemachineCamera focusVCam;  // Kéo thả Focus Virtual Camera (DefeatedEnemyFocusVCam) vào đây
    [SerializeField] private CinemachineBrain cinemachineBrain; // Kéo thả Cinemachine Brain trên Main Camera vào đây

    [Header("Focus Settings")]
    [SerializeField] private float focusDuration = 2f; // Thời gian focus vào kẻ địch
    [SerializeField] private float focusEaseInTime = 0.2f; // Thời gian chuyển tiếp vào focus
    [SerializeField] private float focusEaseOutTime = 0.3f; // Thời gian chuyển tiếp ra khỏi focus
    [SerializeField] private float timeScaleOnFocus = 0.05f; // Tốc độ thời gian khi focus

    private GameObject tempFocusTarget; // Đối tượng tạm thời mà FocusVCam sẽ theo dõi
    private Coroutine focusCoroutine;
    private int originalPlayerVCamPriority; // Lưu priority gốc của PlayerVCam
    [SerializeField]private Animator playerAnimator;

    void Awake()
    {
        if (cinemachineBrain == null)
        {
            cinemachineBrain = Camera.main?.GetComponent<CinemachineBrain>();
        }
        if (playerVCam != null)
        {
            originalPlayerVCamPriority = playerVCam.Priority; // Lưu priority gốc
        }
        // Đảm bảo focusVCam ban đầu có priority thấp hơn hoặc bị tắt
        if (focusVCam != null)
        {
            focusVCam.Priority = 0; // Hoặc một giá trị thấp hơn PlayerVCam
        }
    }

    // Hàm này sẽ được gọi từ Vector3EventListener khi kẻ địch bị tiêu diệt
    public void StartFocusSequence(GameObject enemyObject)
    {
        if (enemyObject == null) return;

        if (focusCoroutine != null)
        {
            StopCoroutine(focusCoroutine);
        }

        // Truyền cả GameObject của enemy và Animator của nó (nếu có)
        focusCoroutine = StartCoroutine(DoFocusSequence(enemyObject));
    }

    private IEnumerator DoFocusSequence(GameObject targetObject)
    {
      
        Vector3 targetPosition = targetObject.transform.position;
        Animator targetAnimator = targetObject.GetComponent<Animator>();

        if (tempFocusTarget == null)
        {
            tempFocusTarget = new GameObject("TempFocusTarget");
        }
        tempFocusTarget.transform.position = targetPosition;

        // 2. Gán Target và kích hoạt FocusVCam
        if (focusVCam != null)
        {
            // Vô hiệu hóa CameraFollow nếu nó vẫn đang hoạt động để tránh xung đột
            playerVCam.enabled = false;
            focusVCam.transform.position = targetPosition;
            // Gán target cho FocusVCam (nếu sử dụng Follow/LookAt)
            focusVCam.LookAt = tempFocusTarget.transform;
            focusVCam.Follow = tempFocusTarget.transform;

            // Tăng Priority của FocusVCam để nó trở thành Active Camera
            focusVCam.Priority = originalPlayerVCamPriority + 1;

            // Đặt blend time để chuyển cảnh mượt mà
            if (cinemachineBrain != null)
            {
                cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.EaseInOut;
                cinemachineBrain.DefaultBlend.Time = focusEaseInTime;
            }
        }

        // 3. Hiệu ứng Slow Motion
        float originalTimeScale = Time.timeScale;
        Time.timeScale = timeScaleOnFocus;

        if (targetAnimator != null)
        {
            targetAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            targetAnimator.speed = timeScaleOnFocus;
        }
        if (playerAnimator != null)
        {
            playerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            playerAnimator.speed = timeScaleOnFocus;
        }
        // Chờ cho quá trình blend camera hoàn tất và giữ focus trong thời gian định trước
        // Sử dụng WaitForSecondsRealtime vì Time.timeScale đã bị thay đổi
        yield return new WaitForSecondsRealtime(focusDuration);

        // 4. Trở lại PlayerVCam
        if (focusVCam != null)
        {
            focusVCam.Priority = originalPlayerVCamPriority - 1; // Giảm priority để PlayerVCam lấy lại quyền điều khiển
            if (cinemachineBrain != null)
            {
                cinemachineBrain.DefaultBlend.Time = focusEaseOutTime; // Thời gian blend trở lại
            }
        }

        // Chờ cho quá trình blend trở lại hoàn tất
        yield return new WaitForSecondsRealtime(focusEaseOutTime);

        // 5. Khôi phục Time Scale
        Time.timeScale = originalTimeScale;
        if (targetAnimator != null)
        {
            targetAnimator.updateMode = AnimatorUpdateMode.Normal;
            targetAnimator.speed = 1f;
        }
        if (playerAnimator != null)
        {
            playerAnimator.updateMode = AnimatorUpdateMode.Normal;
            playerAnimator.speed = 1f;
        }
        // 6. Dọn dẹp
        if (tempFocusTarget != null)
        {
            Destroy(tempFocusTarget); // Xóa đối tượng tạm thời
            tempFocusTarget = null;
        }
        // Bật lại CameraFollow nếu nó bị tắt ban đầu
        if (playerVCam != null && !playerVCam.enabled)
        {
            playerVCam.enabled = true;
        }
    }
}