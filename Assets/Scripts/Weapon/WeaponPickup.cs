using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Pickup")]
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Header("Pickup Settings")]
    [SerializeField] private float pickupRadius = 2f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    [SerializeField] private bool showPickupPrompt = true;
    
    private bool playerInRange = false;
    private WeaponController playerWeaponController;
    
    void Awake()
    {
        // TAMBAHAN: Setup untuk avoid bullet collision
        // Pastikan collider adalah trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        
        // PENTING: Set layer ke "Ignore Raycast" atau custom layer "Pickup"
        // Ini memastikan bullet tidak mendeteksi weapon pickup
        // Jika layer "Pickup" belum ada, fallback ke "Ignore Raycast"
        int pickupLayer = LayerMask.NameToLayer("Pickup");
        if (pickupLayer == -1)
        {
            // Layer "Pickup" belum dibuat, gunakan "Ignore Raycast"
            pickupLayer = LayerMask.NameToLayer("Ignore Raycast");
        }
        
        if (pickupLayer != -1)
        {
            gameObject.layer = pickupLayer;
        }
    }
    
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(pickupKey))
        {
            PickupWeapon();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerWeaponController = other.GetComponent<WeaponController>();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerWeaponController = null;
        }
    }
    
    private void PickupWeapon()
    {
        if (playerWeaponController == null)
        {
            return;
        }
        
        // Unlock weapon berdasarkan type
        switch (weaponType)
        {
            case WeaponType.Rifle:
                playerWeaponController.UnlockRifle();
                break;
            case WeaponType.Shotgun:
                playerWeaponController.UnlockShotgun();
                break;
        }
        
        // Destroy pickup object
        Destroy(gameObject);
    }
    
    private void OnDrawGizmosSelected()
    {
        // Visualisasi pickup radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
    
    // Display pickup prompt (optional - bisa pakai UI canvas)
    private void OnGUI()
    {
        if (playerInRange && showPickupPrompt)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1f);
            GUI.Label(new Rect(screenPos.x - 50, Screen.height - screenPos.y - 20, 100, 20), 
                $"Press {pickupKey} to pickup", 
                new GUIStyle() { alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState() { textColor = Color.white } });
        }
    }
}
