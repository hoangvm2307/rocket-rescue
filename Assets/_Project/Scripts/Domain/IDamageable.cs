using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// Causes the object to take a specified amount of damage and applies a force.
    /// </summary>
    /// <param name="damageAmount">The amount of damage to take.</param>
    /// <param name="hitDirection">The direction from which the damage originated, used for applying force.</param>
    /// <param name="hitForce">The magnitude of the force to apply to the object.</param>
    void TakeDamage(float damageAmount, Vector3 hitDirection, float hitForce); 
} 