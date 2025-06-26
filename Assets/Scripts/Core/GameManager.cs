using UnityEngine;
using UnityEngine.SceneManagement; // Để reload màn chơi
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private float sceneReloadDelay = 2f;

    private int totalEnemies;

    void Awake()
    {
        // Thiết lập Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        // Đếm tổng số kẻ địch có trong màn chơi lúc bắt đầu
        totalEnemies = FindObjectsOfType<BaseEnemy>().Length;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    // Hàm này sẽ được gọi từ script Enemy mỗi khi có 1 kẻ địch bị tiêu diệt
    public void OnEnemyDefeated()
    {
        totalEnemies--;
    }
    // Hàm này để kiểm tra xem đã hết kẻ địch chưa
    public bool AreAllEnemiesDefeated()
    {
        return totalEnemies <= 0;
    }

    public void CheckWinCondition()
    {
        if (AreAllEnemiesDefeated())
        {
            WinGame();
        }
        else
        {
            Debug.Log("Hostage reached, but not all enemies are defeated!");
            // Optionally, show a UI message to the player here.
        }
    }

    private void WinGame()
    {
        Debug.Log("YOU WIN!");
        winPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void HandlePlayerDeath()
    {
        Debug.Log("GameManager received player death event. Starting reload coroutine.");
        losePanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        StartCoroutine(ReloadSceneAfterDelay());
    }

    private IEnumerator ReloadSceneAfterDelay()
    {
        // We need to wait using unscaled time because we set timeScale to 0
        yield return new WaitForSecondsRealtime(sceneReloadDelay);
        
        // Reset time scale before loading the new scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}