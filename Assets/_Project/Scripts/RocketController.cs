using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;
[RequireComponent(typeof(Rigidbody))]
public class RocketController : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private GameObject explosionVFX;

    [Header("Audio")]
    [Tooltip("Kênh sự kiện để yêu cầu phát SFX")]
    [SerializeField] private AudioCueEventChannelSO sfxEventChannel;
    [Tooltip("Âm thanh sẽ phát khi nổ")]
    [SerializeField] private AudioCueSO explosionSound;
    [SerializeField] private AudioCueSO launchSound;


    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    }

    public void Launch(Vector3 direction, float force)
    {
        if (sfxEventChannel != null && launchSound != null)
        {
            sfxEventChannel.RaiseEvent(launchSound);
        }
        rb.AddForce(direction * force, ForceMode.Impulse);

        transform.DOShakeRotation(duration: 5f, strength: 5f, vibrato: 5).SetLoops(-1, LoopType.Incremental);
        // -------------------------------
    }

    void OnCollisionEnter(Collision collision)
    {
        Shield shield = collision.gameObject.GetComponent<Shield>();

        if (shield != null)
        {
            // shield.HandleHit();

            Debug.Log("Rocket hit a shield. Shield is now detached and falling!");

            Destroy(gameObject);
            Destroy(shield.gameObject);

            return;
        }

        Explode();
    }

    private void Explode()
    {
        if (sfxEventChannel != null && explosionSound != null)
        {
            sfxEventChannel.RaiseEvent(explosionSound);
        }

        transform.DOKill();
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CinemachineBrain brain = mainCamera.GetComponent<CinemachineBrain>();
            // Chỉ thực hiện nếu có CinemachineBrain và có một VCam đang hoạt động
            if (brain != null && brain.ActiveVirtualCamera != null)
            {
                // Lấy VCam đang active
                var activeVCam = brain.ActiveVirtualCamera as CinemachineCamera;
                if (activeVCam != null)
                {
                    // Lấy giá trị FOV hiện tại
                    float originalFOV = activeVCam.Lens.FieldOfView;
                    float punchFOV = originalFOV + 3f; // Tăng FOV lên 8 đơn vị

                    // Dùng DOTween để tween giá trị FOV
                    // Chúng ta sẽ dùng DOVirtual vì ta chỉ muốn tween một giá trị float
                    DOVirtual.Float(originalFOV, punchFOV, 0.15f, (value) =>
                    {
                        // Trong mỗi frame của tween, cập nhật FOV của VCam
                        activeVCam.Lens.FieldOfView = value;
                    }).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        // Sau khi zoom ra, tween ngược trở lại giá trị ban đầu
                        DOVirtual.Float(punchFOV, originalFOV, 0.4f, (value) =>
                        {
                            activeVCam.Lens.FieldOfView = value;
                        });
                    });
                }
            }

            // Hiệu ứng rung màn hình cũ vẫn có thể giữ lại
            if (mainCamera.GetComponent<ScreenShake>() != null)
            {
                mainCamera.GetComponent<ScreenShake>().TriggerShake(0.1f, 0.1f);
            }
        }
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerMotor>()?.ApplyRocketJump(transform.position, explosionForce);
            }

            // --- THAY ĐỔI LOGIC GÂY SÁT THƯƠNG ---
            // Tìm kiếm component IDamageable trên đối tượng va phải (hoặc cha của nó)
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                if (!hit.CompareTag("Player"))
                {
                    Vector3 hitDirection = (hit.transform.position - transform.position).normalized;
                    if (hitDirection.y < 0) hitDirection.y = 0;
                    damageable.TakeDamage(damage, hitDirection, explosionForce);
                }
            }
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}