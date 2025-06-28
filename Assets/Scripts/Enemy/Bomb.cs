using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private GameObject explosionVFX;  
 

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    { 
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }
 
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            hit.GetComponent<IDamageable>()?.TakeDamage(damage);
        }
 
        // Destroy(gameObject);
    }
 
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}