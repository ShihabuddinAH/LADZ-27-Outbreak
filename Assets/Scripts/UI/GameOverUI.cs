using UnityEngine;
using TMPro;

/// <summary>
/// UI untuk Game Over Panel - display stats saat game over.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ZombieSpawner spawner;
    [SerializeField] private bool autoFindSpawner = true;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI waveSurvivedText;
    [SerializeField] private TextMeshProUGUI zombiesKilledText;
    [SerializeField] private TextMeshProUGUI survivalTimeText;
    [SerializeField] private TextMeshProUGUI highScoreText; // Optional - tampilkan high score
    
    [Header("Format Settings")]
    [SerializeField] private string waveTextFormat = "Wave Survived: {0}";
    [SerializeField] private string killsTextFormat = "Zombies Killed: {0}";
    [SerializeField] private string timeTextFormat = "Survival Time: {0}";
    [SerializeField] private string highScoreFormat = "Best Wave: {0}";
    
    void Start()
    {
        // Auto-find spawner jika belum di-assign
        if (spawner == null && autoFindSpawner)
        {
            spawner = FindObjectOfType<ZombieSpawner>();
        }
    }
    
    void OnEnable()
    {
        // Update stats setiap kali panel muncul (game over)
        UpdateStats();
    }
    
    public void UpdateStats()
    {
        if (spawner == null)
        {
            Debug.LogWarning("GameOverUI: Spawner not found!");
            return;
        }
        
        // Get stats dari spawner
        int currentWave = spawner.GetCurrentWave();
        int totalSpawned = spawner.GetTotalZombiesSpawned();
        int activeZombies = spawner.GetActiveZombies();
        int zombiesKilled = totalSpawned - activeZombies;
        float gameTime = spawner.GetGameTime();
        
        // Update UI
        if (waveSurvivedText != null)
        {
            waveSurvivedText.text = string.Format(waveTextFormat, currentWave);
        }
        
        if (zombiesKilledText != null)
        {
            zombiesKilledText.text = string.Format(killsTextFormat, zombiesKilled);
        }
        
        if (survivalTimeText != null)
        {
            survivalTimeText.text = string.Format(timeTextFormat, FormatTime(gameTime));
        }
        
        // Update high score (optional)
        if (highScoreText != null)
        {
            int highestWave = PlayerPrefs.GetInt("HighestWave", 0);
            highScoreText.text = string.Format(highScoreFormat, highestWave);
        }
    }
    
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
