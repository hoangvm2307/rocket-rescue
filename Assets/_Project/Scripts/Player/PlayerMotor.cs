// File: PlayerMotor.cs
using UnityEngine;
using DG.Tweening; // << THÊM DÒNG NÀY để sử dụng DOTween

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private Rigidbody rb;
    
    // --- PHẦN MỚI: CÀI ĐẶT HIỆU ỨNG JUICY ---
    [Header("Juicy Effects (DOTween)")]
    [Tooltip("Transform của model nhân vật sẽ bị co giãn. Nếu để trống, nó sẽ lấy transform của chính đối tượng này.")]
    [SerializeField] private Transform playerVisualsTransform; 
    [Tooltip("Độ mạnh của hiệu ứng giãn ra theo chiều dọc.")]
    [SerializeField] private float punchStrengthY = 0.5f;
    [Tooltip("Độ mạnh của hiệu ứng nén lại theo chiều ngang.")]
    [SerializeField] private float punchStrengthX = -0.2f;
    [Tooltip("Thời gian để hoàn thành hiệu ứng co giãn.")]
    [SerializeField] private float punchDuration = 0.4f;
    // -----------------------------------------

    [Header("Rocket Jump Settings")]
    [Tooltip("How much to favor vertical movement. 0 = pure physics, 1 = strong bias upwards.")]
    [Range(0, 1)]
    [SerializeField] private float verticalBias = 0.3f;
    [Tooltip("Ensures a minimum upward push, even if the explosion is level with the player. Prevents horizontal-only movement.")]
    [SerializeField] private float minimumUpwardNudge = 0.2f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints =
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Nếu không gán transform cho model, tự động lấy của chính nó
        if (playerVisualsTransform == null)
        {
            playerVisualsTransform = this.transform;
        }
    }

    public void ApplyRocketJump(Vector3 explosionPosition, float force)
    {
        // --- PHẦN MỚI: KÍCH HOẠT HIỆU ỨNG ---
        // Dừng bất kỳ hiệu ứng co giãn nào đang chạy trước đó để tránh xung đột
        playerVisualsTransform.DOKill(); 
        // Kích hoạt hiệu ứng "punch" vào scale
        playerVisualsTransform.DOPunchScale(
            new Vector3(punchStrengthX, punchStrengthY, punchStrengthX), // Giãn theo Y, nén theo X và Z
            punchDuration, 
            vibrato: 1, // Số lần rung, 1 là đủ cho hiệu ứng tức thì
            elasticity: 0.1f); // Độ đàn hồi, giá trị nhỏ sẽ làm hiệu ứng "chắc" hơn
        // -------------------------------------

        Vector3 directionVector = transform.position - explosionPosition;
        
        if (directionVector.y < minimumUpwardNudge)
        {
            directionVector.y = minimumUpwardNudge;
        }

        Vector3 originalDirection = directionVector.normalized;
        originalDirection.z = 0;
        float verticality = Vector3.Dot(originalDirection, Vector3.up);
        verticality = Mathf.Clamp01(verticality);

        Vector3 biasedDirection = Vector3.Lerp(originalDirection, Vector3.up, verticality * verticalBias);

        rb.AddForce(biasedDirection * force, ForceMode.Impulse);
    }
}