// File: DroneEnemy.cs
// Folder: Scripts/Enemy/
using UnityEngine;

public class DroneEnemy : BaseEnemy
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float moveSpeed = 4f;

    [Header("Combat Settings")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Transform dropPoint; 
    [SerializeField] private float dropCooldown = 2f; 
    [SerializeField] private LayerMask playerLayer; 

    private int currentPointIndex = 0;
    private float lastDropTime;

    void Update()
    {
        HandlePatrol();
        TryToDropBomb();
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

    private void TryToDropBomb()
    {
       
        if (Time.time < lastDropTime + dropCooldown) return;
        
       
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f, playerLayer))
        {
           
            DropBomb();
            lastDropTime = Time.time; 
        }
    }

    private void DropBomb()
    {
        if (bombPrefab != null && dropPoint != null)
        {
            Instantiate(bombPrefab, dropPoint.position, Quaternion.identity);
        }
    }
}