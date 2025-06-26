using UnityEngine;
using UnityEngine.SceneManagement; // Để reload màn chơi

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
        totalEnemies = FindObjectsOfType<Enemy>().Length;
    }

    // Hàm này sẽ được gọi từ script Enemy mỗi khi có 1 kẻ địch bị tiêu diệt
    public void OnAnEnemyDefeated()
    {
        totalEnemies--;
    }
    // Hàm này để kiểm tra xem đã hết kẻ địch chưa
    public bool AreAllEnemiesDefeated()
    {
        return totalEnemies <= 0;
    }

    public void WinGame()
    {
        Debug.Log("YOU WIN! RESCUED THE HOSTAGE!");
        // Tạm thời reload lại màn chơi sau 2 giây
        Invoke("ReloadScene", 2f);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}