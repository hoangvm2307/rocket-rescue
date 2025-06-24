using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 5f; // Bán kính nổ
    [SerializeField] private float explosionForce = 700f; // Lực nổ

    [Header("Lifecycle")]
    [SerializeField] private float lifeTime = 4f; // Thời gian tự hủy nếu không va chạm

    void Start()
    {
        // Tự hủy sau một khoảng thời gian
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
        Destroy(gameObject);
    }

    private void Explode()
    {
        // Tìm tất cả các đối tượng trong bán kính nổ
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
 
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }

    // Vẽ bán kính nổ trong Editor để dễ căn chỉnh
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}