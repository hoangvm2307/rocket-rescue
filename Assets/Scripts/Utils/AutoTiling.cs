// File: Assets/Scripts/Utils/AutoTiling.cs
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class AutoTiling : MonoBehaviour
{
    private static MaterialPropertyBlock propertyBlock;

    void Awake()
    {
        ApplyTiling();
    }

    private void ApplyTiling()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null) return;

        // Khởi tạo propertyBlock nếu chưa có
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        // Lấy các thuộc tính hiện tại của renderer vào block (để không ghi đè các giá trị khác)
        rend.GetPropertyBlock(propertyBlock);

        // Lấy scale của vật thể
        Vector3 objectScale = transform.lossyScale;

        // "_MainTex_ST" là tên thuộc tính shader cho Tiling (ST) của Main Texture
        // S = Scale (Tiling), T = Translate (Offset)
        // Chúng ta chỉ cần set Tiling, nên offset giữ nguyên (0, 0)
        Vector4 tilingAndOffset = new Vector4(objectScale.x, objectScale.y, 0, 0);
        propertyBlock.SetVector("_MainTex_ST", tilingAndOffset);

        // Áp dụng khối thuộc tính đã sửa đổi trở lại renderer
        rend.SetPropertyBlock(propertyBlock);
    }
}