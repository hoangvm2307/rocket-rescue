using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] protected float maxHealth = 1f;
    [SerializeField] protected GameObject deathEffect;
    [SerializeField] private GameObjectEvent onEnemyDefeatedEventWithGameObject;

    [Header("Ragdoll Settings")]
    [Tooltip("Gán tất cả các component Rigidbody từ các khớp xương của ragdoll vào đây.")]
    [SerializeField] private Rigidbody[] ragdollRigidbodies;
    [Tooltip("Animator chính của enemy. Nó sẽ bị vô hiệu hóa khi ragdoll được kích hoạt.")]
    [SerializeField] protected Animator animator;
    [SerializeField] private GameEvent onEnemyDefeatedEvent;
    protected float currentHealth;
    private bool isDead = false;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        SetRagdollEnabled(false);

        foreach (var rb in ragdollRigidbodies)
        {
            if (rb != null)
            {
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }
    }

    public virtual void TakeDamage(float damageAmount, Vector3 hitDirection, float hitForce)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die(hitDirection, hitForce);
        }
    }

    protected virtual void Die(Vector3 hitDirection, float hitForce)
    {
        if (isDead) return;
        isDead = true;

        onEnemyDefeatedEventWithGameObject?.Raise(gameObject);
        onEnemyDefeatedEvent?.Raise();

        SetRagdollEnabled(true);

        Rigidbody centralRigidbody = ragdollRigidbodies.Length > 0 ? ragdollRigidbodies[0] : null;
        if (centralRigidbody != null)
        {
            centralRigidbody.AddForce(hitDirection * hitForce * 10, ForceMode.Impulse);
        }

        if (deathEffect != null)
        {
            Instantiate(deathEffect, centralRigidbody != null ? centralRigidbody.position : transform.position, Quaternion.identity);
        }

        if (TryGetComponent<EnemyPatrol>(out var patrolScript))
        {
            patrolScript.enabled = false;
        }

        if (TryGetComponent<Collider>(out var mainCollider))
        {
            mainCollider.enabled = false;
        }
    }
    public bool IsDead()
    {
        return isDead;
    }
    private void SetRagdollEnabled(bool isEnabled)
    {
        Debug.Log("SetRagdollEnabled: " + isEnabled);
        if (animator != null)
        {
            animator.enabled = !isEnabled;
        }

        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = !isEnabled;
            rb.detectCollisions = isEnabled;
        }
    }
}