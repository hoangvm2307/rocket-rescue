// File: BackgroundColorChanger.cs
using UnityEngine;
using DG.Tweening; // Thêm thư viện DOTween

public class BackgroundColorChanger : MonoBehaviour
{
    [Header("Cài đặt màu sắc")]
    [Tooltip("Danh sách các màu mà background sẽ chuyển đổi qua.")]
    [SerializeField] private Color[] colorPalette;

    [Tooltip("Thời gian để chuyển từ màu này sang màu khác.")]
    [SerializeField] private float transitionDuration = 2f;

    private Renderer backgroundRenderer;
    private int colorIndex = 0;

    void Start()
    {
        // Lấy component Renderer của chính đối tượng này
        backgroundRenderer = GetComponent<Renderer>();

        if (backgroundRenderer == null)
        {
            Debug.LogError("Đối tượng này không có component Renderer.");
            return;
        }

        if (colorPalette == null || colorPalette.Length == 0)
        {
            Debug.LogError("Vui lòng thêm ít nhất một màu vào Color Palette.");
            return;
        }

        // Bắt đầu với màu đầu tiên trong danh sách
        backgroundRenderer.material.color = colorPalette[0];

        // Bắt đầu chu trình đổi màu
        ChangeToNextColor();
    }

    void ChangeToNextColor()
    {
        // Tăng chỉ số màu, và quay vòng lại nếu hết danh sách
        colorIndex = (colorIndex + 1) % colorPalette.Length;

        // Dùng DOTween để chuyển màu của material một cách mượt mà
        backgroundRenderer.material.DOColor(colorPalette[colorIndex], transitionDuration)
            .SetEase(Ease.Linear) // Chuyển màu đều
            .OnComplete(ChangeToNextColor); // Khi hoàn thành, gọi lại chính hàm này để tạo vòng lặp
    }
    
    // (Tùy chọn) Dừng hiệu ứng khi đối tượng bị hủy
    void OnDestroy()
    {
        if (backgroundRenderer != null)
        {
            backgroundRenderer.material.DOKill();
        }
    }
}