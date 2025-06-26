using UnityEngine;

public class EnemyPatrol : BaseEnemy
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;  
    [SerializeField] private float moveSpeed = 3f;

    private int currentPointIndex = 0;

    void Update()
    {
        HandlePatrol();
    }

    private void HandlePatrol()
    {
        if (patrolPoints.Length == 0) return;
 
        Transform targetPoint = patrolPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
 
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }
}