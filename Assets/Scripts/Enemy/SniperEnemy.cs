// File: SniperEnemy.cs
// Folder: Scripts/Enemy/
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SniperEnemy : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private LayerMask obstacleLayer; // Layer của tường, nền đất...

    [Header("Combat")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float aimDuration = 1.5f;
    [SerializeField] private float fireCooldown = 3f;

    private LineRenderer laserSight;
    private bool canFire = true;

    void Awake()
    {
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

        // Kiểm tra xem có nhìn thấy người chơi không
        if (Vector3.Distance(transform.position, playerTransform.position) <= detectionRange &&
            !Physics.Raycast(firePoint.position, directionToPlayer, directionToPlayer.magnitude, obstacleLayer))
        {
            // Nếu thấy, bắt đầu ngắm bắn
            StartCoroutine(AimAndFire(directionToPlayer));
        }
    }

    private IEnumerator AimAndFire(Vector3 direction)
    {
        canFire = false; // Ngăn việc gọi Coroutine này nhiều lần

        // --- Giai đoạn ngắm ---
        laserSight.enabled = true;
        laserSight.SetPosition(0, firePoint.position);
        laserSight.SetPosition(1, playerTransform.position);

        // Cập nhật vị trí laser theo người chơi
        float timer = 0;
        while (timer < aimDuration)
        {
            laserSight.SetPosition(1, playerTransform.position);
            timer += Time.deltaTime;
            yield return null;
        }

        laserSight.enabled = false;

        // --- Giai đoạn bắn ---
        // Xoay nòng súng về hướng người chơi trước khi bắn
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // --- Giai đoạn hồi chiêu ---
        yield return new WaitForSeconds(fireCooldown);
        canFire = true;
    }
}