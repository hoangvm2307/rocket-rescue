using UnityEngine;

public class Hostage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.AreAllEnemiesDefeated())
            {
                GameManager.Instance.WinGame();
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Kill all enemies before rescuing the hostage!");

            }
        }
    }
}