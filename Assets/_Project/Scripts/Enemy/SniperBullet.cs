using UnityEngine;

public class SniperBullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float hitForce = 10f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector3 hitDirection = transform.forward;
                damageable.TakeDamage(damage, hitDirection, hitForce);
            }
            Destroy(gameObject);
        }
    }
}