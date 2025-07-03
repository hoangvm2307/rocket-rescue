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
    private int totalHostages;
    private int enemiesDefeated = 0;
    private int hostagesRescued = 0;

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
        totalEnemies = FindObjectsOfType<BaseEnemy>().Length;
        totalHostages = FindObjectsOfType<Hostage>().Length;

        winPanel.SetActive(false);
        losePanel.SetActive(false);

        UpdateObjectivesUI();
    }
    public void OnEnemyDefeated()
    {
        enemiesDefeated++;
        UpdateAndCheckWinCondition();
    }
    public void OnHostageRescued()
    {
        hostagesRescued++;
        UpdateAndCheckWinCondition();
    }
    private void UpdateAndCheckWinCondition()
    {
        UpdateObjectivesUI();
        CheckWinCondition();
    }
    // Hàm này để kiểm tra xem đã hết kẻ địch chưa
    public bool AreAllEnemiesDefeated()
    {
        return totalEnemies <= 0;
    }

    // Hàm này để kiểm tra xem đã giải cứu hết hostage chưa
    public bool AreAllHostagesRescued()
    {
        return hostagesRescued >= totalHostages;
    }

    private void CheckWinCondition()
    {
        // Kiểm tra điều kiện thắng dựa trên các biến đếm
        if (enemiesDefeated >= totalEnemies && hostagesRescued >= totalHostages)
        {
            WinGame();
        }
    }

    private void UpdateObjectivesUI()
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            int enemiesRemaining = totalEnemies - enemiesDefeated;
            int hostagesRemaining = totalHostages - hostagesRescued;
            uiManager.UpdateObjectives(enemiesRemaining, hostagesRemaining);
        }
    }

    private void WinGame()
    {
        Debug.Log("YOU WIN!");
        winPanel.SetActive(true);
        // Time.timeScale = 0f; // Pause the game
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