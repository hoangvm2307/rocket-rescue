// File: Scripts/Player/PlayerMotor.cs
using UnityEngine;
using DG.Tweening;
// Không cần thư viện "System.Collections" nữa

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private Rigidbody rb;
    
    [Header("Juicy Effects (DOTween)")]
    [SerializeField] private Transform playerVisualsTransform; 
    [SerializeField] private float punchStrengthY = 0.5f;
    [SerializeField] private float punchStrengthX = -0.2f;
    [SerializeField] private float punchDuration = 0.4f;

    [Header("Rocket Jump Settings")]
    [SerializeField] private float verticalBias = 0.3f;
    [SerializeField] private float minimumUpwardNudge = 0.2f;

    // Biến để lưu trữ platform mà người chơi đang đứng trên
    private MovingPlatform currentPlatform;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints =
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (playerVisualsTransform == null)
        {
            playerVisualsTransform = this.transform;
        }
    }

    // Sử dụng FixedUpdate để xử lý vật lý
    void FixedUpdate()
    {
        if (currentPlatform != null)
        {
            // Di chuyển người chơi cùng với platform
            // Bằng cách cộng trực tiếp vận tốc của platform vào vị trí của người chơi
            rb.MovePosition(rb.position + currentPlatform.Velocity * Time.fixedDeltaTime);
        }
    }

    public void ApplyRocketJump(Vector3 explosionPosition, float force)
    {
        // VÌ ĐÃ BỎ SETPARENT, CHÚNG TA KHÔNG CẦN COROUTINE NỮA
        // Có thể gọi trực tiếp DOPunchScale một cách an toàn
        playerVisualsTransform.DOKill(); 
        playerVisualsTransform.DOPunchScale(
            new Vector3(punchStrengthX, punchStrengthY, punchStrengthX),
            punchDuration, 
            vibrato: 1,
            elasticity: 0.1f);
        
        // Logic vật lý của Rocket Jump giữ nguyên
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
    
    // Phát hiện khi nào người chơi tiếp xúc với platform
    private void OnCollisionEnter(Collision collision)
    {
        // Thử lấy component MovingPlatform từ đối tượng va chạm
        if (collision.gameObject.TryGetComponent<MovingPlatform>(out MovingPlatform platform))
        {
            // Nếu có, lưu nó lại
            currentPlatform = platform;
        }
    }

    // Phát hiện khi nào người chơi rời khỏi platform
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<MovingPlatform>(out MovingPlatform platform))
        {
            // Chỉ xóa tham chiếu nếu đó đúng là platform người chơi vừa rời đi
            if (platform == currentPlatform)
            {
                currentPlatform = null;
            }
        }
    }
}