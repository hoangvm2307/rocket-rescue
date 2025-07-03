using UnityEngine;

[CreateAssetMenu(fileName = "NewRocketType", menuName = "Rocket Rescue/Rocket Type")]
public class RocketTypeSO : ScriptableObject
{
    [Header("Info")]
    public string rocketName;
    public Sprite icon;

    [Header("Gameplay")]
    public GameObject rocketPrefab; 
    public int maxAmmo;
    public bool isInfinite = false;
}