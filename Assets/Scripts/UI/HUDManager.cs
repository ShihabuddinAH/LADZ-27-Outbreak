using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// HUD UI - Display score, kills, time, wave, weapon, ammo.
/// </summary>
public class HUDManager : MonoBehaviour
{
    [Header("Score & Stats")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI waveText;
    
    [Header("Weapon Info")]
    [SerializeField] private Image weaponIcon; // Gambar senjata
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI reloadingText; // "RELOADING..."
    
    [Header("Health")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    
    [Header("References")]
    [SerializeField] private GameStats gameStats;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private PlayerHealth playerHealth;
    
    [Header("Format Settings")]
    [SerializeField] private string scoreFormat = "?? {0}";
    [SerializeField] private string killsFormat = "?? {0}";
    [SerializeField] private string waveFormat = "Wave: {0}";
    [SerializeField] private string ammoFormat = "{0} / {1}";
    [SerializeField] private string timeFormat = "{0}";
    
    void Start()
    {
        // Auto-find references using newer API
        if (gameStats == null) gameStats = FindFirstObjectByType<GameStats>();
        if (weaponController == null) weaponController = FindFirstObjectByType<WeaponController>();
        if (playerHealth == null) playerHealth = FindFirstObjectByType<PlayerHealth>();
        
        // Subscribe to events
        if (gameStats != null)
        {
            gameStats.OnScoreChanged += UpdateScore;
            gameStats.OnKillsChanged += UpdateKills;
            gameStats.OnTimeChanged += UpdateTime;
            gameStats.OnWaveChanged += UpdateWave;
        }
        
        // Hide reloading text
        if (reloadingText != null)
        {
            reloadingText.gameObject.SetActive(false);
        }
        
        // Initial update
        UpdateAllUI();
    }
    
    void Update()
    {
        // Update weapon & ammo info setiap frame (bisa optimize dengan event)
        UpdateWeaponInfo();
        UpdateHealthBar();
    }
    
    private void UpdateAllUI()
    {
        if (gameStats != null)
        {
            UpdateScore(gameStats.GetScore());
            UpdateKills(gameStats.GetKills());
            UpdateTime(gameStats.GetSurvivalTime());
            UpdateWave(gameStats.GetCurrentWave());
        }
    }
    
    private void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = string.Format(scoreFormat, score);
        }
    }
    
    private void UpdateKills(int kills)
    {
        if (killsText != null)
        {
            killsText.text = string.Format(killsFormat, kills);
        }
    }
    
    private void UpdateTime(float time)
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            string formattedTime = $"{minutes:00}:{seconds:00}";
            timeText.text = string.Format(timeFormat, formattedTime);
        }
    }
    
    private void UpdateWave(int wave)
    {
        if (waveText != null)
        {
            waveText.text = string.Format(waveFormat, wave);
        }
    }
    
    private void UpdateWeaponInfo()
    {
        if (weaponController == null) return;
        
        WeaponData weapon = weaponController.GetCurrentWeapon();
        if (weapon == null) return;
        
        // Update weapon icon
        if (weaponIcon != null && weapon.weaponIcon != null)
        {
            weaponIcon.sprite = weapon.weaponIcon;
            weaponIcon.gameObject.SetActive(true);
        }
        
        // Update weapon name
        if (weaponNameText != null)
        {
            weaponNameText.text = weapon.weaponName.ToUpper();
        }
        
        // Update ammo
        if (ammoText != null)
        {
            int currentAmmo = weaponController.GetCurrentAmmo();
            ammoText.text = string.Format(ammoFormat, currentAmmo, weapon.magazineSize);
            
            // Color coding untuk low ammo
            if (currentAmmo <= weapon.magazineSize * 0.3f)
            {
                ammoText.color = Color.red;
            }
            else if (currentAmmo <= weapon.magazineSize * 0.5f)
            {
                ammoText.color = Color.yellow;
            }
            else
            {
                ammoText.color = Color.white;
            }
        }
        
        // Update reloading status
        if (reloadingText != null)
        {
            bool isReloading = weaponController.IsReloading();
            reloadingText.gameObject.SetActive(isReloading);
        }
    }
    
    private void UpdateHealthBar()
    {
        if (playerHealth == null) return;
        
        // Update health bar
        if (healthBar != null)
        {
            healthBar.maxValue = playerHealth.GetMaxHealth();
            healthBar.value = playerHealth.GetCurrentHealth();
        }
        
        // Update health text
        if (healthText != null)
        {
            healthText.text = $"{playerHealth.GetCurrentHealth()} / {playerHealth.GetMaxHealth()}";
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (gameStats != null)
        {
            gameStats.OnScoreChanged -= UpdateScore;
            gameStats.OnKillsChanged -= UpdateKills;
            gameStats.OnTimeChanged -= UpdateTime;
            gameStats.OnWaveChanged -= UpdateWave;
        }
    }
}
