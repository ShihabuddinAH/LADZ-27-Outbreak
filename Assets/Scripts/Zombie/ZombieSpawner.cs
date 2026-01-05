using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject zombiePrefab; // Prefab zombie yang akan di-spawn
    [SerializeField] private Transform player; // Reference ke player
    [SerializeField] private bool autoFindPlayer = true; // Otomatis cari player saat start
    
    [Header("Spawn Distance")]
    [SerializeField] private float minSpawnDistance = 10f; // Jarak minimum dari player
    [SerializeField] private float maxSpawnDistance = 20f; // Jarak maksimum dari player
    
    [Header("Spawn Area Restriction")]
    [SerializeField] private bool useRestrictedArea = true; // Enable area restriction
    [SerializeField] private float restrictedMinX = -20f; // Jangan spawn di dalam area X ini
    [SerializeField] private float restrictedMaxX = 20f;
    [SerializeField] private float restrictedMinY = -12f; // Jangan spawn di dalam area Y ini
    [SerializeField] private float restrictedMaxY = 12f;
    [SerializeField] private int maxSpawnAttempts = 20; // Maksimal percobaan untuk cari posisi valid
    
    [Header("Spawn Timing")]
    [SerializeField] private float initialSpawnDelay = 2f; // Delay awal sebelum spawn pertama
    [SerializeField] private float baseSpawnInterval = 3f; // Base interval waktu antara spawn (detik)
    [SerializeField] private int baseZombiesPerWave = 1; // Jumlah zombie per wave di awal
    
    [Header("Difficulty Scaling")]
    [SerializeField] private bool enableDifficultyScaling = true; // Enable peningkatan difficulty
    [SerializeField] private float difficultyIncreaseInterval = 30f; // Setiap berapa detik difficulty naik
    [SerializeField] private int maxZombiesPerWave = 10; // Maksimal zombie per wave
    [SerializeField] private float minSpawnInterval = 1f; // Interval spawn tercepat
    
    [Header("Spawn Limits")]
    [SerializeField] private int maxActiveZombies = 50; // Maksimal zombie aktif di scene
    [SerializeField] private bool limitActiveZombies = true; // Limit zombie yang aktif
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true; // Tampilkan info di console
    [SerializeField] private bool drawSpawnZone = true; // Visualisasi zona spawn di editor
    [SerializeField] private bool drawRestrictedArea = true; // Visualisasi restricted area
    
    private float gameTime = 0f;
    private int currentWave = 0;
    private int totalZombiesSpawned = 0;
    private bool isSpawning = false;
    
    // Difficulty scaling values (tidak langsung ubah serialized fields)
    private int currentZombiesPerWave = 1;
    private float currentSpawnInterval = 3f;
    private int lastDifficultyLevel = 0;
    
    void Start()
    {
        // Auto-find player jika belum di-assign
        if (player == null && autoFindPlayer)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                if (showDebugInfo)
                {
                    Debug.Log("ZombieSpawner: Player found automatically!");
                }
            }
            else
            {
                Debug.LogError("ZombieSpawner: Player not found! Make sure player has tag 'Player'");
                enabled = false;
                return;
            }
        }
        
        if (zombiePrefab == null)
        {
            Debug.LogError("ZombieSpawner: Zombie Prefab not assigned!");
            enabled = false;
            return;
        }
        
        // Initialize difficulty values
        currentZombiesPerWave = baseZombiesPerWave;
        currentSpawnInterval = baseSpawnInterval;
        
        // Mulai spawning setelah initial delay
        StartCoroutine(StartSpawning());
    }
    
    void Update()
    {
        gameTime += Time.deltaTime;
        
        // Update difficulty seiring waktu
        if (enableDifficultyScaling)
        {
            UpdateDifficulty();
        }
    }
    
    private IEnumerator StartSpawning()
    {
        // Delay awal
        yield return new WaitForSeconds(initialSpawnDelay);
        
        if (showDebugInfo)
        {
            Debug.Log("ZombieSpawner: Starting endless spawn!");
        }
        
        isSpawning = true;
        
        // Loop spawning endless
        while (isSpawning)
        {
            // Cek limit zombie aktif
            if (!CanSpawnMoreZombies())
            {
                if (showDebugInfo)
                {
                    Debug.Log("ZombieSpawner: Waiting for zombies to die before spawning more...");
                }
                yield return new WaitForSeconds(1f); // Tunggu 1 detik sebelum cek lagi
                continue;
            }
            
            // Spawn wave
            SpawnWave();
            
            // PENTING: Gunakan currentSpawnInterval (bukan baseSpawnInterval)
            // Tapi jangan langsung reference variable yang bisa berubah di tengah coroutine
            float waitTime = currentSpawnInterval;
            yield return new WaitForSeconds(waitTime);
        }
    }
    
    private void SpawnWave()
    {
        currentWave++;
        
        if (showDebugInfo)
        {
            Debug.Log($"ZombieSpawner: Spawning Wave {currentWave} ({currentZombiesPerWave} zombies)");
        }
        
        for (int i = 0; i < currentZombiesPerWave; i++)
        {
            SpawnZombie();
        }
    }
    
    private void SpawnZombie()
    {
        if (player == null || zombiePrefab == null)
        {
            return;
        }
        
        // Generate posisi spawn random di sekitar player
        Vector2 spawnPosition = GetRandomSpawnPosition();
        
        // Spawn zombie
        GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
        
        if (zombie != null)
        {
            totalZombiesSpawned++;
            
            if (showDebugInfo)
            {
                Debug.Log($"ZombieSpawner: Zombie #{totalZombiesSpawned} spawned at {spawnPosition}");
            }
        }
    }
    
    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 spawnPosition = Vector2.zero;
        int attempts = 0;
        bool validPosition = false;
        
        while (!validPosition && attempts < maxSpawnAttempts)
        {
            // Generate random angle (0-360 derajat)
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            
            // Generate random distance antara min dan max
            float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
            
            // Hitung posisi spawn berdasarkan angle dan distance
            Vector2 offset = new Vector2(
                Mathf.Cos(randomAngle) * randomDistance,
                Mathf.Sin(randomAngle) * randomDistance
            );
            
            spawnPosition = (Vector2)player.position + offset;
            
            // Check apakah posisi valid (di luar restricted area)
            if (useRestrictedArea)
            {
                validPosition = IsPositionOutsideRestrictedArea(spawnPosition);
            }
            else
            {
                validPosition = true;
            }
            
            attempts++;
        }
        
        if (!validPosition && showDebugInfo)
        {
            Debug.LogWarning($"ZombieSpawner: Could not find valid spawn position after {maxSpawnAttempts} attempts. Using last attempt.");
        }
        
        return spawnPosition;
    }
    
    private bool IsPositionOutsideRestrictedArea(Vector2 position)
    {
        // Check apakah posisi DI LUAR restricted area
        // Spawn hanya jika X atau Y di luar batas
        bool outsideX = position.x < restrictedMinX || position.x > restrictedMaxX;
        bool outsideY = position.y < restrictedMinY || position.y > restrictedMaxY;
        
        // Valid jika salah satu (atau kedua) di luar batas
        return outsideX || outsideY;
    }
    
    private bool CanSpawnMoreZombies()
    {
        if (!limitActiveZombies)
        {
            return true;
        }
        
        // Hitung jumlah zombie aktif
        int activeZombies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        
        if (activeZombies >= maxActiveZombies)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"ZombieSpawner: Max zombies reached ({activeZombies}/{maxActiveZombies}). Waiting...");
            }
            return false;
        }
        
        return true;
    }
    
    private void UpdateDifficulty()
    {
        // Hitung difficulty level berdasarkan waktu
        int difficultyLevel = Mathf.FloorToInt(gameTime / difficultyIncreaseInterval);
        
        // Hanya update jika difficulty level berubah
        if (difficultyLevel == lastDifficultyLevel)
        {
            return;
        }
        
        lastDifficultyLevel = difficultyLevel;
        
        // Increase zombies per wave
        int newZombiesPerWave = baseZombiesPerWave + difficultyLevel;
        newZombiesPerWave = Mathf.Min(newZombiesPerWave, maxZombiesPerWave);
        
        if (newZombiesPerWave != currentZombiesPerWave)
        {
            currentZombiesPerWave = newZombiesPerWave;
            
            if (showDebugInfo)
            {
                Debug.Log($"ZombieSpawner: Difficulty increased! Zombies per wave: {currentZombiesPerWave}");
            }
        }
        
        // Decrease spawn interval (faster spawning)
        float newSpawnInterval = baseSpawnInterval - (difficultyLevel * 0.2f);
        newSpawnInterval = Mathf.Max(newSpawnInterval, minSpawnInterval);
        
        if (newSpawnInterval != currentSpawnInterval)
        {
            currentSpawnInterval = newSpawnInterval;
            
            if (showDebugInfo)
            {
                Debug.Log($"ZombieSpawner: Spawn interval decreased to {currentSpawnInterval:F1}s");
            }
        }
    }
    
    // Method untuk spawn zombie secara manual (bisa dipanggil dari luar)
    public void SpawnZombieManual()
    {
        SpawnZombie();
    }
    
    public void SpawnMultipleZombies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnZombie();
        }
    }
    
    // Method untuk pause/resume spawning
    public void PauseSpawning()
    {
        isSpawning = false;
        if (showDebugInfo)
        {
            Debug.Log("ZombieSpawner: Spawning paused");
        }
    }
    
    public void ResumeSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(StartSpawning());
            if (showDebugInfo)
            {
                Debug.Log("ZombieSpawner: Spawning resumed");
            }
        }
    }
    
    // Getters untuk info spawner
    public int GetCurrentWave()
    {
        return currentWave;
    }
    
    public int GetTotalZombiesSpawned()
    {
        return totalZombiesSpawned;
    }
    
    public int GetActiveZombies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
    
    public float GetGameTime()
    {
        return gameTime;
    }
    
    public bool IsSpawning()
    {
        return isSpawning;
    }
    
    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        if (!drawSpawnZone)
        {
            return;
        }
        
        // Cari player jika belum ada
        Transform targetPlayer = player;
        if (targetPlayer == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                targetPlayer = playerObj.transform;
            }
        }
        
        if (targetPlayer == null)
        {
            return;
        }
        
        // Draw spawn zone (lingkaran kuning dan merah)
        Gizmos.color = Color.yellow;
        DrawCircle(targetPlayer.position, minSpawnDistance, 32);
        
        Gizmos.color = Color.red;
        DrawCircle(targetPlayer.position, maxSpawnDistance, 32);
        
        // Draw restricted area (kotak biru)
        if (useRestrictedArea && drawRestrictedArea)
        {
            Gizmos.color = new Color(0, 0, 1, 0.3f); // Blue semi-transparent
            
            Vector3 center = new Vector3(
                (restrictedMinX + restrictedMaxX) / 2f,
                (restrictedMinY + restrictedMaxY) / 2f,
                0f
            );
            
            Vector3 size = new Vector3(
                restrictedMaxX - restrictedMinX,
                restrictedMaxY - restrictedMinY,
                0.1f
            );
            
            Gizmos.DrawCube(center, size);
            
            // Draw border
            Gizmos.color = Color.blue;
            DrawRectangle(restrictedMinX, restrictedMaxX, restrictedMinY, restrictedMaxY);
        }
    }
    
    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 previousPoint = center + new Vector3(radius, 0, 0);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Gizmos.DrawLine(previousPoint, newPoint);
            previousPoint = newPoint;
        }
    }
    
    private void DrawRectangle(float minX, float maxX, float minY, float maxY)
    {
        Vector3 bottomLeft = new Vector3(minX, minY, 0);
        Vector3 bottomRight = new Vector3(maxX, minY, 0);
        Vector3 topRight = new Vector3(maxX, maxY, 0);
        Vector3 topLeft = new Vector3(minX, maxY, 0);
        
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}
