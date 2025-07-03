using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Ragdoll")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody[] ragdollRigidbodies;

    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        // Ban đầu, tắt hết vật lý của ragdoll đi
        SetRagdollEnabled(false);
    }

    // Hàm này đến từ Interface IDamageable
    public void TakeDamage(float damage, Vector3 hitDirection, float hitForce)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die(hitDirection, hitForce);
        }
    }

    private void Die(Vector3 hitDirection, float hitForce)
    {
        isDead = true;

        // Kích hoạt ragdoll
        SetRagdollEnabled(true);

        // Tìm một xương trung tâm (ví dụ: xương chậu) để tác động lực
        Rigidbody centralRigidbody = ragdollRigidbodies.Length > 0 ? ragdollRigidbodies[0] : null;
        if (centralRigidbody != null)
        {
            centralRigidbody.AddForce(hitDirection * hitForce, ForceMode.Impulse);
        }

        // Tắt các component không cần thiết nữa
        GetComponent<EnemyPatrol>().enabled = false;
        // Tắt collider chính của enemy để không cản đường đạn nữa
        GetComponent<Collider>().enabled = false;
    }

    private void SetRagdollEnabled(bool isEnabled)
    {
        // Bật/tắt Animator
        animator.enabled = !isEnabled;

        // Bật/tắt isKinematic trên tất cả các xương
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = !isEnabled;
        }
    }

    public void TakeDamage(float damageAmount)
    {
         
    }
}