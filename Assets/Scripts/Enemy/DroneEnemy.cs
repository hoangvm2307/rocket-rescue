// File: DroneEnemy.cs
// Folder: Scripts/Enemy/
using UnityEngine;

[RequireComponent(typeof(LineRenderer))] // Ensure the component is always there
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
    private LineRenderer targetingRay; // Reference to the Line Renderer

    protected override void Awake()
    {
        base.Awake();
        targetingRay = GetComponent<LineRenderer>();
        targetingRay.positionCount = 2; // The ray has a start and an end
    }

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
        float raycastDistance = 100f;
        RaycastHit hit;
        
        // Always update the starting point of the ray
        targetingRay.SetPosition(0, transform.position);

        if (Time.time >= lastDropTime + dropCooldown && Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, playerLayer))
        {
            // Set end point and color when detecting player
            targetingRay.SetPosition(1, hit.point);
            targetingRay.startColor = targetingRay.endColor = Color.green;

            DropBomb();
            lastDropTime = Time.time;
        }
        else
        {
            // Set end point and color when not detecting or on cooldown
            targetingRay.SetPosition(1, transform.position + Vector3.down * raycastDistance);
            targetingRay.startColor = targetingRay.endColor = Color.red;
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