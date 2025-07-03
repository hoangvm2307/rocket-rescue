using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private GameEvent onPlayerDied;

    [Header("Ragdoll Components")]
    [Tooltip("Animator chính của Player")]
    [SerializeField] private Animator animator;
    [Tooltip("Rigidbody chính của Player")]
    [SerializeField] private Rigidbody mainRigidbody;
    [Tooltip("Collider chính của Player")]
    [SerializeField] private Collider mainCollider;
    [Tooltip("Gán tất cả Rigidbody của các khớp xương ragdoll vào đây")]
    [SerializeField] private Rigidbody[] ragdollRigidbodies;

    [Header("Components to Disable on Death")]
    [SerializeField] private PlayerMotor playerMotor;
    [SerializeField] private PlayerWeapon playerWeapon;
    [SerializeField] private PlayerInput playerInput;

    private float currentHealth;
    public bool IsDead { get; private set; }

    void Awake()
    {
        currentHealth = maxHealth;
        SetRagdollEnabled(false);
    }

    public void TakeDamage(float damageAmount, Vector3 hitDirection, float hitForce)
    {
        if (IsDead) return;

        currentHealth -= damageAmount;
        Debug.Log($"Player took {damageAmount} damage, health is now {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0; 
            Die(hitDirection, hitForce);
        }
    }
    private void SetRagdollEnabled(bool isEnabled)
    {
        Debug.Log("[PlayerHealth] SetRagdollEnabled: " + isEnabled);
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
    private void Die(Vector3 hitDirection, float hitForce)
    {
        if (IsDead) return;
        IsDead = true;
 
        SetRagdollEnabled(true);
 
        Rigidbody centralRigidbody = ragdollRigidbodies.Length > 0 ? ragdollRigidbodies[0] : null;
        if (centralRigidbody != null)
        { 
            centralRigidbody.AddForce(hitDirection * hitForce * 10, ForceMode.Impulse);
        }
 
        Debug.Log("Player has died. Raising OnPlayerDied event.");
        onPlayerDied?.Raise(); 
    }
}