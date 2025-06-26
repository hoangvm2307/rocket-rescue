using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    [Header("Base Enemy Settings")]
    [SerializeField] protected float maxHealth = 1f; // Default to 1, as most enemies die in one hit.
    [SerializeField] protected GameObject deathEffect;
    [SerializeField] private GameEvent onEnemyDefeatedEvent;

    protected float currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        onEnemyDefeatedEvent?.Raise();

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
} 