using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer lineRenderer;


    [Header("Aiming IK")]
    [SerializeField] private Transform aimTarget;

    [Header("Settings")]
    [SerializeField] private float rocketLaunchForce = 20f;
    [SerializeField] private float maxDragDistance = 400f;

    [Header("Trajectory Prediction")]
    [SerializeField] private int trajectoryPointCount = 50;
    [SerializeField] private float trajectoryTimeStep = 0.1f;
    [Tooltip("The spacing between dots on the trajectory line.")]
    [SerializeField] private float trajectoryDotSpacing = 0.5f;

    [Header("Events")]
    [SerializeField] private IntEvent onAmmoChanged;

    [Header("Weapon Inventory")]
    [SerializeField] private List<RocketTypeSO> availableRocketTypes;
    private Dictionary<RocketTypeSO, int> ammoInventory = new Dictionary<RocketTypeSO, int>();
    private int currentRocketIndex = 0;

    // Thay vì lưu vị trí thế giới, ta lưu vị trí màn hình
    private Vector3 dragStartScreenPos;

    [Header("Aiming & Firing")]
    [Tooltip("How far in front the aiming target is placed.")]
    [SerializeField] private float aimDistance = 15f;
    [Tooltip("Controls how much the drag angle affects the launch angle.")]
    [SerializeField] private float angleSensitivity = 0.25f;
    [Tooltip("The curve that maps normalized drag distance (0-1) to weapon power (0-1). Allows for non-linear power scaling.")]
    [SerializeField] private AnimationCurve dragToPowerCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [Header("Juicy Effects")]
    [SerializeField] private Transform weaponModelTransform;
    private RocketTypeSO CurrentRocketType => availableRocketTypes.Count > 0 ? availableRocketTypes[currentRocketIndex] : null;
    public int CurrentAmmo => CurrentRocketType != null ? ammoInventory[CurrentRocketType] : 0;
    public bool HasInfiniteAmmo => CurrentRocketType != null && CurrentRocketType.isInfinite;

    void Start()
    {
        foreach (var rocketType in availableRocketTypes)
        {
            ammoInventory[rocketType] = rocketType.maxAmmo;
        }

        lineRenderer.enabled = false;
        // This is needed to make the texture for the line tile, creating a dotted effect.
        lineRenderer.textureMode = LineTextureMode.Tile;
        RaiseAmmoChangedEvent();

    }

    public void HandleDragStart(Vector3 screenPosition)
{
    dragStartScreenPos = screenPosition;
    lineRenderer.enabled = true;

    // --- THÊM HIỆU ỨNG XUẤT HIỆN ---
    // Giả sử đường đạn có màu trắng
    Color originalColor = Color.white; 
    // Bắt đầu với màu trong suốt
    Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0); 
    
    // Gán màu ban đầu
    lineRenderer.startColor = transparentColor;
    lineRenderer.endColor = transparentColor;

    // Dùng DOTween để làm màu hiện dần lên
    lineRenderer.DOColor(
        new Color2(transparentColor, transparentColor), // Màu bắt đầu
        new Color2(originalColor, originalColor),      // Màu kết thúc
        0.2f);                                         // Thời gian
    // -------------------------------
    
    HandleDrag(screenPosition);
}

    public void HandleDrag(Vector3 screenPosition)
    {
        Vector3 dragVector = screenPosition - dragStartScreenPos;

        // --- 1. Player Rotation ---
        // If dragging right (dragVector.x > 0), player faces left (-90 degrees)
        // If dragging left (dragVector.x < 0), player faces right (90 degrees)
        if (dragVector.x > 1f) // Use a small threshold to avoid flickering
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (dragVector.x < -1f)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }

        // --- 2. Calculate Aim Angle ---
        // We use Atan2 to find the angle of the drag. The angle is INVERTED for the "pull back to shoot" feel.
        // The horizontal component for Atan2 is the absolute drag distance, vertical is the drag height.
        float horizontalDrag = Mathf.Abs(dragVector.x);
        float verticalDrag = -dragVector.y; // Invert Y so dragging down aims up.

        float launchAngle = Mathf.Atan2(verticalDrag, horizontalDrag) * Mathf.Rad2Deg;

        // --- 3. Position the Aim Target ---
        // Start with the player's forward direction (which is now set correctly)
        Vector3 forwardDirection = transform.forward;

        // To pitch the aim up/down correctly, we need to rotate around a horizontal axis
        // that is perpendicular to our forward direction. We get this with a Cross Product.
        // This is much safer than using transform.right, which can be unreliable after complex rotations.
        Vector3 pitchAxis = Vector3.Cross(forwardDirection, Vector3.up);

        // Apply the pitch rotation around our safely calculated axis
        Quaternion pitchRotation = Quaternion.AngleAxis(launchAngle * angleSensitivity, pitchAxis);

        // Apply the pitch to get the final direction
        Vector3 finalDirection = pitchRotation * forwardDirection;

        // Place the target
        aimTarget.position = transform.position + finalDirection * aimDistance;

        // --- 4. Update Visualizer with Trajectory Prediction ---
        float screenDragMagnitude = (dragStartScreenPos - screenPosition).magnitude;
        screenDragMagnitude = Mathf.Min(screenDragMagnitude, maxDragDistance);
        float normalizedDrag = screenDragMagnitude / maxDragDistance;
        float power = dragToPowerCurve.Evaluate(normalizedDrag);

        Vector3 fireDirection = (aimTarget.position - firePoint.position).normalized;
        Vector3 initialVelocity = fireDirection * rocketLaunchForce * power;

        // Calculate the trajectory points using the physics formula
        Vector3[] points = new Vector3[trajectoryPointCount];
        float time = 0f;
        for (int i = 0; i < trajectoryPointCount; i++)
        {
            // Projectile motion formula: P(t) = P0 + V0*t + 0.5*g*t^2
            points[i] = firePoint.position + initialVelocity * time + Physics.gravity * 0.5f * time * time;
            time += trajectoryTimeStep;
        }

        UpdateAimVisualizer(points);
    }

    public void HandleDragEnd(Vector3 screenPosition)
    {
        lineRenderer.enabled = false;

        float screenDragMagnitude = (dragStartScreenPos - screenPosition).magnitude;

        screenDragMagnitude = Mathf.Min(screenDragMagnitude, maxDragDistance);

        float normalizedDrag = screenDragMagnitude / maxDragDistance;

        float power = dragToPowerCurve.Evaluate(normalizedDrag);

        if (power > 0.05f)
        {
            Vector3 fireDirection = (aimTarget.position - firePoint.position).normalized;
            FireRocket(fireDirection, power);
        }
    }

    private void FireRocket(Vector3 direction, float power)
    {
        if (CurrentRocketType == null) return;

        if (!HasInfiniteAmmo)
        {
            if (CurrentAmmo <= 0) return;
            ammoInventory[CurrentRocketType]--;
        }

        RaiseAmmoChangedEvent();

        // --- THÊM HIỆU ỨNG RECOIL ---
        if (weaponModelTransform != null)
        {
            weaponModelTransform.DOKill(); // Dừng hiệu ứng cũ
                                           // Giật lùi về sau một chút
            weaponModelTransform.DOPunchPosition(-transform.forward * 0.2f, 0.15f);
        }
        // ----------------------------

        Quaternion rocketRotation = Quaternion.LookRotation(-direction);
        GameObject rocketGO = Instantiate(CurrentRocketType.rocketPrefab, firePoint.position, rocketRotation);
        rocketGO.GetComponent<RocketController>().Launch(direction, rocketLaunchForce * power);
    }
    private void UpdateAimVisualizer(Vector3[] points)
    {
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);

        // --- This section is for creating the dotted line effect ---
        // 1. Calculate the total length of the line by summing the distances between points
        float totalLength = 0f;
        for (int i = 0; i < points.Length - 1; i++)
        {
            totalLength += Vector3.Distance(points[i], points[i + 1]);
        }

        // 2. Adjust the material's texture tiling based on the length
        // This makes the dot texture repeat along the line, creating the dotted effect.
        if (trajectoryDotSpacing > 0)
        {
            lineRenderer.material.mainTextureScale = new Vector2(totalLength / trajectoryDotSpacing, 1f);
        }
    }

    private void RaiseAmmoChangedEvent()
    {
        if (onAmmoChanged == null) return;

        if (HasInfiniteAmmo)
        {
            onAmmoChanged.Raise(-1); // Use -1 to signify infinite ammo
        }
        else
        {
            onAmmoChanged.Raise(CurrentAmmo);
        }
    }
}