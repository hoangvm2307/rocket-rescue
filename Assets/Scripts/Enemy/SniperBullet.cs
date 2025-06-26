
using UnityEngine;

public class SniperBullet : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER HIT BY SNIPER!");
        }

        if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}