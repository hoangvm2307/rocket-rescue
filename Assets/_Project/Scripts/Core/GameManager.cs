using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private float sceneReloadDelay = 2f;
    [SerializeField] private float deathPanelDelay = 2f;
    private int totalEnemies;
    private int totalHostages;
    private int enemiesDefeated = 0;
    private int hostagesRescued = 0;

    void Awake()
    {
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
    public bool AreAllEnemiesDefeated()
    {
        return totalEnemies <= 0;
    }

    public bool AreAllHostagesRescued()
    {
        return hostagesRescued >= totalHostages;
    }

    private void CheckWinCondition()
    {
        if (enemiesDefeated >= totalEnemies && hostagesRescued >= totalHostages)
        {
            StartCoroutine(HandleWinGame());
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

    private IEnumerator HandleWinGame()
    {
        yield return new WaitForSeconds(1.5f);
        winPanel.SetActive(true); 
    }

    public void HandlePlayerDeath()
    {
        StartCoroutine(PlayerDeathSequence());
    }

    private IEnumerator ReloadSceneAfterDelay()
    {
        yield return new WaitForSecondsRealtime(sceneReloadDelay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private IEnumerator PlayerDeathSequence()
    {
        // A. Đợi trong một khoảng thời gian
        yield return new WaitForSeconds(deathPanelDelay);

        // B. Sau khi đợi xong, thực hiện các hành động thua cuộc
        losePanel.SetActive(true);
        Time.timeScale = 0f; 
    }
}