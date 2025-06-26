using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameEvent onEnemyDefeatedEvent;
    public GameObject deathEffect; // Hiệu ứng khi chết (tùy chọn)

    // Hàm này sẽ được gọi bởi quả rocket khi nó nổ gần kẻ địch
    public void TakeDamage()
    {
        onEnemyDefeatedEvent.Raise();

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Phá hủy đối tượng kẻ địch
        Destroy(gameObject);
    }
}