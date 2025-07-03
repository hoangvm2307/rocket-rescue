// File: SelfRotate.cs
using UnityEngine;
using DG.Tweening; // << Bắt buộc phải có dòng này để dùng DOTween

public class SelfRotate : MonoBehaviour
{
    [Header("Cài đặt xoay")]
    [Tooltip("Tốc độ xoay. Số lớn hơn sẽ xoay nhanh hơn.")]
    [SerializeField] private float rotationDuration = 5f;

    [Tooltip("Hướng xoay. Ví dụ (0, 360, 0) để xoay quanh trục Y.")]
    [SerializeField] private Vector3 rotationDirection = new Vector3(0, 360, 0);

    void Start()
    {
        // Bắt đầu hiệu ứng xoay
        AnimateRotation();
    }

    void AnimateRotation()
    {
        // Dùng DOLocalRotate để xoay đối tượng theo trục cục bộ của nó
        // Hiệu ứng sẽ lặp lại vô tận (SetLoops(-1))
        // RotateMode.LocalAxisAdd đảm bảo nó xoay liên tục thay vì chỉ xoay đến một góc rồi dừng
        transform.DOLocalRotate(rotationDirection, rotationDuration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear) // Xoay đều, không nhanh không chậm
            .SetLoops(-1); // Lặp lại vô tận
    }

    // (Tùy chọn) Dừng hiệu ứng khi đối tượng bị hủy để dọn dẹp
    void OnDestroy()
    {
        transform.DOKill();
    }
}