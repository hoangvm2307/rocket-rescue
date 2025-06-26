// File: UIManager.cs
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;

    /// <summary>
    /// Public method to be called by an IntEventListener.
    /// It updates the ammo text based on the received value.
    /// </summary>
    /// <param name="ammoCount">The current ammo count. -1 signifies infinite ammo.</param>
    public void UpdateAmmoText(int ammoCount)
    {
        if (ammoText != null)
        {
            if (ammoCount < 0) // Check for the infinite ammo signal
            {
                ammoText.text = "ROCKETS: ∞";
            }
            else
            {
                ammoText.text = $"ROCKETS: {ammoCount}";
            }
        }
    }
}