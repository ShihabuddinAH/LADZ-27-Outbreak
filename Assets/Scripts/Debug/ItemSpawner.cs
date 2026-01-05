using UnityEngine;

/// <summary>
/// Debug script untuk spawn item secara manual
/// Gunakan untuk testing item pickup system
/// </summary>
public class ItemSpawner : MonoBehaviour
{
    [Header("Item Prefabs")]
    [SerializeField] private GameObject medkitPrefab;
    [SerializeField] private GameObject handgunAmmoPrefab;
    [SerializeField] private GameObject rifleAmmoPrefab;
    [SerializeField] private GameObject shotgunAmmoPrefab;
    
    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 3f;
    [SerializeField] private bool spawnAtMousePosition = false;
    
    [Header("Debug Keys")]
    [SerializeField] private KeyCode spawnMedkitKey = KeyCode.F1;
    [SerializeField] private KeyCode spawnHandgunAmmoKey = KeyCode.F2;
    [SerializeField] private KeyCode spawnRifleAmmoKey = KeyCode.F3;
    [SerializeField] private KeyCode spawnShotgunAmmoKey = KeyCode.F4;
    [SerializeField] private KeyCode spawnRandomKey = KeyCode.F5;
    
    void Update()
    {
        if (Input.GetKeyDown(spawnMedkitKey))
        {
            SpawnItem(medkitPrefab);
        }
        
        if (Input.GetKeyDown(spawnHandgunAmmoKey))
        {
            SpawnItem(handgunAmmoPrefab);
        }
        
        if (Input.GetKeyDown(spawnRifleAmmoKey))
        {
            SpawnItem(rifleAmmoPrefab);
        }
        
        if (Input.GetKeyDown(spawnShotgunAmmoKey))
        {
            SpawnItem(shotgunAmmoPrefab);
        }
        
        if (Input.GetKeyDown(spawnRandomKey))
        {
            SpawnRandomItem();
        }
    }
    
    private void SpawnItem(GameObject itemPrefab)
    {
        if (itemPrefab == null)
        {
            Debug.LogWarning("Item prefab not assigned!");
            return;
        }
        
        Vector3 spawnPos = GetSpawnPosition();
        Instantiate(itemPrefab, spawnPos, Quaternion.identity);
        
        Debug.Log($"Spawned {itemPrefab.name} at {spawnPos}");
    }
    
    private void SpawnRandomItem()
    {
        GameObject[] items = { medkitPrefab, handgunAmmoPrefab, rifleAmmoPrefab, shotgunAmmoPrefab };
        
        // Filter null
        System.Collections.Generic.List<GameObject> validItems = new System.Collections.Generic.List<GameObject>();
        foreach (GameObject item in items)
        {
            if (item != null)
            {
                validItems.Add(item);
            }
        }
        
        if (validItems.Count == 0)
        {
            Debug.LogWarning("No item prefabs assigned!");
            return;
        }
        
        GameObject randomItem = validItems[Random.Range(0, validItems.Count)];
        SpawnItem(randomItem);
    }
    
    private Vector3 GetSpawnPosition()
    {
        if (spawnAtMousePosition)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            return mousePos;
        }
        else
        {
            // Random position around this object
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            return transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!spawnAtMousePosition)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
