using UnityEngine;
using TMPro;

/// <summary>
/// UI untuk display weapon info (ammo, weapon name).
/// Auto-update setiap frame.
/// </summary>
public class WeaponUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private bool autoFindWeaponController = true;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI reloadingText; // Optional - tampilkan "RELOADING..."
    
    [Header("Update Settings")]
    [SerializeField] private float updateInterval = 0.1f; // Update UI setiap 0.1 detik
    
    private float lastUpdateTime = 0f;
    
    void Start()
    {
        // Auto-find weapon controller jika belum di-assign
        if (weaponController == null && autoFindWeaponController)
        {
            weaponController = FindObjectOfType<WeaponController>();
            
            if (weaponController == null)
            {
                Debug.LogWarning("WeaponUI: WeaponController not found!");
            }
        }
        
        // Hide reloading text di awal
        if (reloadingText != null)
        {
            reloadingText.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        if (weaponController == null)
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
        UpdateAmmoText();
        UpdateWeaponName();
        UpdateReloadingStatus();
    }
    
    private void UpdateAmmoText()
    {
        if (ammoText == null) return;
        
        int currentAmmo = weaponController.GetCurrentAmmo();
        WeaponData weapon = weaponController.GetCurrentWeapon();
        
        if (weapon != null)
        {
            // Format: "30 / 30"
            ammoText.text = $"{currentAmmo} / {weapon.magazineSize}";
            
            // Optional: Change color jika ammo rendah
            if (currentAmmo <= weapon.magazineSize * 0.3f)
            {
                ammoText.color = Color.red; // Warning - ammo rendah
            }
            else if (currentAmmo <= weapon.magazineSize * 0.5f)
            {
                ammoText.color = Color.yellow; // Caution
            }
            else
            {
                ammoText.color = Color.white; // Normal
            }
        }
    }
    
    private void UpdateWeaponName()
    {
        if (weaponNameText == null) return;
        
        WeaponData weapon = weaponController.GetCurrentWeapon();
        
        if (weapon != null)
        {
            weaponNameText.text = weapon.weaponName.ToUpper();
        }
    }
    
    private void UpdateReloadingStatus()
    {
        if (reloadingText == null) return;
        
        bool isReloading = weaponController.IsReloading();
        reloadingText.gameObject.SetActive(isReloading);
    }
}
