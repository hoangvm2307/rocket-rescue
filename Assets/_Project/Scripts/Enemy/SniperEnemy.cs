using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SniperEnemy : BaseEnemy
{
    [Header("Targeting")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Combat")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float aimDuration = 1.5f;
    [SerializeField] private float fireCooldown = 3f;

    private LineRenderer laserSight;
    private bool canFire = true;

    protected override void Awake()
    {
        base.Awake();
        laserSight = GetComponent<LineRenderer>();
        laserSight.enabled = false;

        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (playerTransform == null || !canFire) return;

        Vector3 directionToPlayer = playerTransform.position - firePoint.position;

        if (Vector3.Distance(transform.position, playerTransform.position) <= detectionRange &&
            !Physics.Raycast(firePoint.position, directionToPlayer, directionToPlayer.magnitude, obstacleLayer))
        {
            StartCoroutine(AimAndFire(directionToPlayer));
        }
    }

    private IEnumerator AimAndFire(Vector3 direction)
    {
        canFire = false;

        laserSight.enabled = true;
        laserSight.SetPosition(0, firePoint.position);
        laserSight.SetPosition(1, playerTransform.position);

        float timer = 0;
        while (timer < aimDuration)
        {
            laserSight.SetPosition(1, playerTransform.position);
            timer += Time.deltaTime;
            yield return null;
        }

        laserSight.enabled = false;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        yield return new WaitForSeconds(fireCooldown);
        canFire = true;
    }
}