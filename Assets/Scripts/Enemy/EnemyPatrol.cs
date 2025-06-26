using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;  
    [SerializeField] private float moveSpeed = 3f;

    [Header("Combat Settings")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameEvent onEnemyDefeatedEvent;

    private int currentPointIndex = 0;

    void Update()
    {
        if (patrolPoints.Length == 0) return;
 
        Transform targetPoint = patrolPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
 
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }
     
    public void TakeDamage()
    {
        if (onEnemyDefeatedEvent != null)
        {
            onEnemyDefeatedEvent.Raise();
        }

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}