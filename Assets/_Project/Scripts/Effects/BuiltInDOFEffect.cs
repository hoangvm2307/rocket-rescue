using UnityEngine;

// Các attribute này giúp script chạy trong editor và hiển thị trong Scene view
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[ImageEffectAllowedInSceneView]
public class BuiltInDOFEffect : MonoBehaviour
{
    public Shader dofShader; // Kéo file shader vào đây
    
    // Các biến điều khiển sẽ hiện ra trong Inspector
    [Range(0.1f, 100f)]
    public float focusDistance = 10f;

    [Range(0f, 100f)]
    public float focusRange = 3f;

    [Range(0f, 5f)]
    public float blurAmount = 1f;

    private Material dofMaterial;
    private Camera mainCamera;

    void OnEnable()
    {
        // Bật depth texture cho camera này
        mainCamera = GetComponent<Camera>();
        mainCamera.depthTextureMode |= DepthTextureMode.Depth;
    }

    // OnRenderImage là hàm đặc biệt của Built-in RP
    // nó được gọi sau khi camera render xong scene
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (dofShader == null)
        {
            Graphics.Blit(source, destination); // Nếu không có shader, chỉ copy ảnh gốc
            return;
        }

        // Tạo Material nếu chưa có
        if (dofMaterial == null)
        {
            dofMaterial = new Material(dofShader);
            dofMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        // Gửi các giá trị từ Inspector vào trong shader
        dofMaterial.SetFloat("_FocusDistance", focusDistance);
        dofMaterial.SetFloat("_FocusRange", focusRange);
        dofMaterial.SetFloat("_BlurAmount", blurAmount);

        // Chạy shader: lấy ảnh nguồn (source), xử lý bằng material, và xuất ra đích (destination)
        Graphics.Blit(source, destination, dofMaterial);
    }
}