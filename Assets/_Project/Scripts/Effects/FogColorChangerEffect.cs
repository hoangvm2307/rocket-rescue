using UnityEngine;

public class FogColorChanger : MonoBehaviour
{
    // Mảng chứa các màu bạn muốn Fog chuyển đổi qua
    public Color[] colors;

    // Thời gian để chuyển từ màu này sang màu kế tiếp (tính bằng giây)
    public float transitionDuration = 5.0f;

    private int colorIndex = 0;
    private float t = 0; // Biến đếm thời gian cho việc chuyển màu
    private Color startColor;

    void Start()
    {
        // Kiểm tra xem mảng màu có rỗng không
        if (colors.Length == 0)
        {
            Debug.LogError("Vui lòng thêm ít nhất một màu vào mảng Colors trong Inspector.");
            this.enabled = false; // Vô hiệu hóa script nếu không có màu nào
            return;
        }

        // Thiết lập màu Fog ban đầu
        RenderSettings.fogColor = colors[0];
        startColor = RenderSettings.fogColor;
    }

    void Update()
    {
        // Dùng Color.Lerp để chuyển đổi màu một cách mượt mà
        // t / transitionDuration sẽ tạo ra một giá trị từ 0 đến 1
        RenderSettings.fogColor = Color.Lerp(startColor, colors[colorIndex], t / transitionDuration);

        // Tăng biến đếm thời gian
        t += Time.deltaTime;

        // Khi quá trình chuyển màu hoàn tất (khi t >= transitionDuration)
        if (t >= transitionDuration)
        {
            // Đặt màu Fog thành màu đích để đảm bảo chính xác
            RenderSettings.fogColor = colors[colorIndex];

            // Reset biến đếm
            t = 0;

            // Lưu lại màu hiện tại để làm màu bắt đầu cho lần chuyển kế tiếp
            startColor = RenderSettings.fogColor;
            
            // Chuyển sang màu tiếp theo trong mảng
            colorIndex++;

            // Nếu đã đi hết mảng màu, quay trở lại vị trí đầu tiên
            if (colorIndex >= colors.Length)
            {
                colorIndex = 0;
            }
        }
    }
}