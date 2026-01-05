using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon System/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponName;
    public WeaponType weaponType;
    
    [Header("Combat Stats")]
    public int damage = 10;
    public float range = 5f;
    public float fireRate = 0.5f; // Cooldown antara tembakan (detik)
    public int magazineSize = 10;
    public float reloadTime = 2f;
    
    [Header("Visual & Audio")]
    public Sprite weaponSprite; // Sprite senjata (optional)
    public Sprite weaponIcon; // TAMBAHAN: Icon untuk UI display
    public string shootAnimationTrigger = "Shoot"; // Nama trigger di Animator
    
    [Header("Weapon Behavior")]
    public bool isAutomatic = false; // True = tahan, False = klik per shot
    public int bulletsPerShot = 1; // Shotgun = 3-5, rifle/handgun = 1
    public float spreadAngle = 0f; // Sudut spread untuk shotgun
}

public enum WeaponType
{
    Handgun,
    Rifle,
    Shotgun,
}
