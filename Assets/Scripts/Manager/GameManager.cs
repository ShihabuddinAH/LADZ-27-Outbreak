using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ZombieSpawner spawner;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameStats gameStats;
    
    [Header("Game State")]
    [SerializeField] private bool isPaused = false;
    [SerializeField] private bool isGameOver = false;
    
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject hudPanel;
    
    private static GameManager instance;
    public static GameManager Instance => instance;
    
    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        if (spawner == null)
            spawner = FindFirstObjectByType<ZombieSpawner>();
        
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        
        if (gameStats == null)
            gameStats = FindFirstObjectByType<GameStats>();
        
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);
        
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
        
        if (!isGameOver && playerHealth != null && playerHealth.GetCurrentHealth() <= 0)
        {
            GameOver();
        }
    }
    
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
        
        if (hudPanel != null)
            hudPanel.SetActive(false);
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        
        if (hudPanel != null)
            hudPanel.SetActive(true);
    }
    
    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Time.timeScale = 0f;
        
        if (spawner != null)
            spawner.enabled = false;
        
        CleanupAllSpawnedObjects();
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        if (hudPanel != null)
            hudPanel.SetActive(false);
        
        SaveStats();
    }
    
    public void RestartGame()
    {
        CleanupAllSpawnedObjects();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitToMainMenu()
    {
        CleanupAllSpawnedObjects();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
    private void CleanupAllSpawnedObjects()
    {
        // Cleanup Items
        ItemPickup[] items = FindObjectsByType<ItemPickup>(FindObjectsSortMode.None);
        foreach (ItemPickup item in items)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        
        // Cleanup Zombies
        ZombieHealth[] zombies = FindObjectsByType<ZombieHealth>(FindObjectsSortMode.None);
        foreach (ZombieHealth zombie in zombies)
        {
            if (zombie != null)
                Destroy(zombie.gameObject);
        }
        
        // Cleanup Bullets
        Bullet[] bullets = FindObjectsByType<Bullet>(FindObjectsSortMode.None);
        foreach (Bullet bullet in bullets)
        {
            if (bullet != null)
                Destroy(bullet.gameObject);
        }
    }
    
    private void SaveStats()
    {
        if (spawner == null) return;
        
        int currentWave = spawner.GetCurrentWave();
        int totalKills = spawner.GetTotalZombiesSpawned() - spawner.GetActiveZombies();
        float survivalTime = spawner.GetGameTime();
        
        int highestWave = PlayerPrefs.GetInt("HighestWave", 0);
        if (currentWave > highestWave)
            PlayerPrefs.SetInt("HighestWave", currentWave);
        
        int totalKillsAllTime = PlayerPrefs.GetInt("TotalKills", 0);
        PlayerPrefs.SetInt("TotalKills", totalKillsAllTime + totalKills);
        
        float longestSurvival = PlayerPrefs.GetFloat("LongestSurvival", 0f);
        if (survivalTime > longestSurvival)
            PlayerPrefs.SetFloat("LongestSurvival", survivalTime);
        
        PlayerPrefs.Save();
    }
    
    // Getters
    public bool IsPaused() => isPaused;
    public bool IsGameOver() => isGameOver;
    public ZombieSpawner GetSpawner() => spawner;
    public GameStats GetGameStats() => gameStats;
}
