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

    // BIẾN MỚI ĐỂ CHỈNH MÀU
    [Tooltip("Màu của tia laser khi ngắm")]
    [SerializeField] private Color laserColor = Color.red;

    private LineRenderer laserSight;
    private bool canFire = true;
    private Coroutine aimingCoroutine;

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
        if (IsDead()) return;
        if (playerTransform == null || !canFire) return;

        Vector3 directionToPlayer = playerTransform.position - firePoint.position;
        bool hasLineOfSight = !Physics.Raycast(firePoint.position, directionToPlayer.normalized, directionToPlayer.magnitude, obstacleLayer);

        if (Vector3.Distance(transform.position, playerTransform.position) <= detectionRange && hasLineOfSight)
        {
            if (aimingCoroutine == null)
            {
                aimingCoroutine = StartCoroutine(AimAndFire());
            }
        }
        else
        {
            if (aimingCoroutine != null)
            {
                StopCoroutine(aimingCoroutine);
                aimingCoroutine = null;
                laserSight.enabled = false;
            }
        }
    }

    private IEnumerator AimAndFire()
    {
        canFire = false;

        laserSight.enabled = true;
        laserSight.SetPosition(0, firePoint.position);

        // --- THAY ĐỔI MÀU SẮC CỦA LASER ---
        laserSight.startColor = laserColor;
        laserSight.endColor = laserColor;
        // ---------------------------------

        float timer = 0;
        while (timer < aimDuration)
        {
            Vector3 directionToPlayer = playerTransform.position - firePoint.position;
            if (Physics.Raycast(firePoint.position, directionToPlayer.normalized, directionToPlayer.magnitude, obstacleLayer))
            {
                laserSight.enabled = false;
                aimingCoroutine = null;
                yield return new WaitForSeconds(fireCooldown);
                canFire = true;
                yield break;
            }

            laserSight.SetPosition(1, playerTransform.position);
            timer += Time.deltaTime;
            yield return null;
        }

        laserSight.enabled = false;

        Vector3 finalDirection = (playerTransform.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        aimingCoroutine = null;

        yield return new WaitForSeconds(fireCooldown);
        canFire = true;
    }
}