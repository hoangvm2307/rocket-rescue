using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
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
            if (hit.CompareTag("Player"))
            {
                Debug.Log("PLAYER HIT BY BOMB!");
                // TODO: Gọi hàm trừ máu hoặc thua game ở đây
                // GameManager.Instance.LoseGame();
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