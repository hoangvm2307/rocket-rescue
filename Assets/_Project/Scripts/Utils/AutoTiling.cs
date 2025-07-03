// File: Assets/Scripts/Utils/AutoTiling.cs
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class AutoTiling : MonoBehaviour
{
    [Tooltip("Hệ số nhân để điều khiển mật độ lặp texture. X tương ứng với trục X của object, Y tương ứng với trục Z.")]
    public Vector2 tilingMultiplier = Vector2.one;

    private Material _materialInstance;

    void Start()
    {
        // Lấy component Renderer và tạo một bản sao material mới từ nó.
        // Điều này ngăn các thay đổi ảnh hưởng đến các đối tượng khác dùng chung material.
        _materialInstance = GetComponent<Renderer>().material;
    }

    void LateUpdate()
    {
        // Tính toán tiling dựa trên local scale của object.
        // Đối với việc tiling trên một mặt phẳng, chúng ta thường sử dụng các thành phần scale X và Z.
        float newTilingX = transform.localScale.x * tilingMultiplier.x;
        float newTilingY = transform.localScale.z * tilingMultiplier.y;

        _materialInstance.mainTextureScale = new Vector2(newTilingX, newTilingY);
    }
}