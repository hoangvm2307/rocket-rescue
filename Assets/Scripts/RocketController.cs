using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RocketController : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private GameObject explosionVFX;

    private Rigidbody rb;
    private GameObject owner;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    }

    public void SetOwner(GameObject ownerObject)
    {
        this.owner = ownerObject;
    }

    public void Launch(Vector3 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);
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
        if (Camera.main.GetComponent<ScreenShake>() != null)
        {
            Camera.main.GetComponent<ScreenShake>().TriggerShake(0.1f, 0.1f);
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

            if (hit.GetComponent<EnemyPatrol>() != null)
            {
                hit.GetComponent<EnemyPatrol>().TakeDamage();
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