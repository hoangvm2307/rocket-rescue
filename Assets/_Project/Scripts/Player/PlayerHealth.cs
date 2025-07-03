using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private GameEvent onPlayerDied;

    private float currentHealth;
    public bool IsDead { get; private set; }

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount, Vector3 hitDirection, float hitForce)
    {
        if (IsDead) return;

        currentHealth -= damageAmount;
        Debug.Log($"Player took {damageAmount} damage, health is now {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died. Raising OnPlayerDied event.");
        onPlayerDied?.Raise();
        gameObject.SetActive(false);
    }
} 