// File: UIManager.cs
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private PlayerWeapon playerWeapon; // Cần tham chiếu để lấy số đạn

    // Hàm này sẽ được gọi bởi GameEventListener khi sự kiện OnAmmoChanged xảy ra
    public void UpdateAmmoText()
    {
        if (playerWeapon != null && ammoText != null)
        {
            if (playerWeapon.HasInfiniteAmmo)
            {
                ammoText.text = "ROCKETS: ∞";
            }
            else
            {
                ammoText.text = $"ROCKETS: {playerWeapon.CurrentAmmo}";
            }
        }
    }
}