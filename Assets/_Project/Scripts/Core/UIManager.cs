// File: UIManager.cs
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    
    [Header("Objectives UI")]
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI hostagesText;

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
    
    /// <summary>
    /// Updates the objectives display with current enemies and hostages count.
    /// </summary>
    /// <param name="enemiesRemaining">Number of enemies left to defeat</param>
    /// <param name="hostagesRemaining">Number of hostages left to rescue</param>
    public void UpdateObjectives(int enemiesRemaining, int hostagesRemaining)
    {
        Debug.Log("UpdateObjectives: " + enemiesRemaining + " " + hostagesRemaining);
        if (enemiesText != null)
        {
            enemiesText.text = $"ENEMIES: {enemiesRemaining}";
            
            // Color code: red if enemies remain, green if all defeated
            enemiesText.color = enemiesRemaining > 0 ? Color.red : Color.green;
        }
        
        if (hostagesText != null)
        {
            hostagesText.text = $"HOSTAGES: {hostagesRemaining}";
            
            // Color code: yellow if hostages remain, green if all rescued
            hostagesText.color = hostagesRemaining > 0 ? Color.yellow : Color.green;
        }
    }
}