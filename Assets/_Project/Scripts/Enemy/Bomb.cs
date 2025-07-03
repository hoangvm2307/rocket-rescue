using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float explosionForce = 500f;

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
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector3 hitDirection = (hit.transform.position - transform.position).normalized;
                damageable.TakeDamage(damage, hitDirection, explosionForce);
            }
        }
 
        Destroy(gameObject);
    }
 
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}