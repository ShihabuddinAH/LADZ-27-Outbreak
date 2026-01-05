using UnityEngine;
using TMPro;

public class SpawnerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ZombieSpawner spawner;
    [SerializeField] private bool autoFindSpawner = true;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI zombiesSpawnedText;
    [SerializeField] private TextMeshProUGUI activeZombiesText;
    [SerializeField] private TextMeshProUGUI gameTimeText;
    [SerializeField] private TextMeshProUGUI zombiesPerWaveText;
    
    [Header("Update Settings")]
    [SerializeField] private float updateInterval = 0.5f; // Update UI setiap 0.5 detik
    
    private float lastUpdateTime = 0f;
    
    void Start()
    {
        // Auto-find spawner jika belum di-assign
        if (spawner == null && autoFindSpawner)
        {
            spawner = FindObjectOfType<ZombieSpawner>();
            
            if (spawner == null)
            {
                Debug.LogWarning("SpawnerUI: ZombieSpawner not found in scene!");
            }
        }
    }
    
    void Update()
    {
        if (spawner == null)
        {
            return;
        }
        
        // Update UI dengan interval tertentu untuk performance
        if (Time.time >= lastUpdateTime + updateInterval)
        {
            UpdateUI();
            lastUpdateTime = Time.time;
        }
    }
    
    private void UpdateUI()
    {
        // Update wave number
        if (waveText != null)
        {
            waveText.text = $"Wave: {spawner.GetCurrentWave()}";
        }
        
        // Update total zombies spawned
        if (zombiesSpawnedText != null)
        {
            zombiesSpawnedText.text = $"Zombies Killed: {spawner.GetTotalZombiesSpawned() - spawner.GetActiveZombies()}";
        }
        
        // Update active zombies
        if (activeZombiesText != null)
        {
            activeZombiesText.text = $"Active Zombies: {spawner.GetActiveZombies()}";
        }
        
        // Update game time
        if (gameTimeText != null)
        {
            float time = spawner.GetGameTime();
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            gameTimeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
}
