// File: AutoFocusDoF.cs
using UnityEngine;
using UnityEngine.Rendering.PostProcessing; // << Thêm thư viện Post Processing

public class AutoFocusDoF : MonoBehaviour
{
    [Tooltip("Kéo đối tượng Player vào đây.")]
    [SerializeField] private Transform target;

    private PostProcessVolume volume;
    private DepthOfField depthOfFieldLayer;

    void Start()
    {
        // Lấy các component cần thiết
        volume = GetComponent<PostProcessVolume>();
        if (volume.profile.TryGetSettings(out depthOfFieldLayer))
        {
            // Lấy được layer Depth of Field thành công
        }
        else
        {
            Debug.LogError("Không tìm thấy Depth of Field trong Post-process Volume Profile.");
        }
    }

    void Update()
    {
        if (target == null || depthOfFieldLayer == null)
            return;

        // Tính khoảng cách từ camera chính đến người chơi
        float distance = Vector3.Distance(Camera.main.transform.position, target.position);

        // Cập nhật giá trị Focus Distance của hiệu ứng trong thời gian thực
        depthOfFieldLayer.focusDistance.value = distance;
    }
}