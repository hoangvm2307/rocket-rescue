using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform cannonTransform;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Settings")]
    [SerializeField] private float rocketLaunchForce = 20f;
    [SerializeField] private float maxDragDistance = 3f;

    [Header("Events")]
    [SerializeField] private IntEvent onAmmoChanged;

    [Header("Weapon Inventory")]
    [SerializeField] private List<RocketTypeSO> availableRocketTypes;
    private Dictionary<RocketTypeSO, int> ammoInventory = new Dictionary<RocketTypeSO, int>();
    private int currentRocketIndex = 0;
    
    // Thay vì lưu vị trí thế giới, ta lưu vị trí màn hình
    private Vector3 dragStartScreenPos;

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
        RaiseAmmoChangedEvent();
     
    }
 
    public void HandleDragStart(Vector3 screenPosition)
    {
        dragStartScreenPos = screenPosition;
        lineRenderer.enabled = true;

        // Cập nhật đường ngắm
        Vector3 direction = (screenPosition - dragStartScreenPos).normalized;
        UpdateAimVisualizer(direction);
    }

    public void HandleDrag(Vector3 screenPosition)
    {
        Vector3 dragVectorScreen = screenPosition - dragStartScreenPos;
        Vector3 direction = dragVectorScreen.normalized;
        
        RotateCannon(-direction);
        UpdateAimVisualizer(dragVectorScreen);
    }
    
    public void HandleDragEnd(Vector3 screenPosition)
    {
        lineRenderer.enabled = false;
        Vector3 dragVectorScreen = screenPosition - dragStartScreenPos;
        
        // Chuyển đổi khoảng cách kéo trên màn hình thành lực bắn
        // Ta có thể dùng một hệ số để điều chỉnh cho phù hợp
        float screenDragMagnitude = dragVectorScreen.magnitude;
        float launchPower = Mathf.Clamp01(screenDragMagnitude / (Screen.height * 0.2f)); // ví dụ: 20% chiều cao màn hình là lực tối đa

        if (launchPower > 0.1f)
        {
            FireRocket(-dragVectorScreen.normalized, launchPower);
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

        Quaternion rocketRotation = cannonTransform.rotation;
        GameObject rocketGO = Instantiate(CurrentRocketType.rocketPrefab, firePoint.position, rocketRotation);
        rocketGO.GetComponent<RocketController>().Launch(direction, rocketLaunchForce * power);
    }

    private void RotateCannon(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        cannonTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void UpdateAimVisualizer(Vector3 dragVectorScreen)
    {
        float dragDistance = dragVectorScreen.magnitude;
        float visualMagnitude = Mathf.Clamp(dragDistance, 0, maxDragDistance * 50); // Nhân với một hệ số để đường kẻ dài hơn trên màn hình
        Vector3 limitedDragVector = dragVectorScreen.normalized * visualMagnitude;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, firePoint.position);
        
        // Vector hướng từ nòng súng, ngược với hướng kéo
        Vector3 aimDirection = -limitedDragVector.normalized;
        
        // Điểm cuối của đường kẻ
        Vector3 endPoint = firePoint.position + (Vector3)aimDirection * (visualMagnitude / 50f); // Chia lại cho hệ số để có độ dài hợp lý trong world space
        lineRenderer.SetPosition(1, endPoint);
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