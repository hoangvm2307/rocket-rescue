using UnityEngine;

public class Hostage : MonoBehaviour
{
    [SerializeField] private GameEvent onAllEnemiesDefeatedAndHostageReached;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // The Hostage's only job is to announce it has been reached.
            // The GameManager will decide if this constitutes a win.
            onAllEnemiesDefeatedAndHostageReached?.Raise();
            
            // Optionally, disable the hostage to prevent re-triggering.
            // Destroy(gameObject) could also work.
            gameObject.SetActive(false);
        }
    }
}