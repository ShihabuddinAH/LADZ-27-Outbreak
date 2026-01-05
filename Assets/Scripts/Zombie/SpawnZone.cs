using UnityEngine;

/// <summary>
/// Zona spawn khusus untuk spawner.
/// Bisa digunakan untuk membuat multiple spawn zones atau spawn zones dengan bentuk custom.
/// </summary>
public class SpawnZone : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private float radius = 15f;
    [SerializeField] private bool isActive = true;
    
    [Header("Spawn Restrictions")]
    [SerializeField] private LayerMask obstacleLayer; // Layer untuk obstacle (wall, dll)
    [SerializeField] private bool checkForObstacles = false; // Check jika ada obstacle
    
    [Header("Visual")]
    [SerializeField] private Color zoneColor = Color.green;
    [SerializeField] private bool showInGame = false;
    
    /// <summary>
    /// Get random position dalam zona ini
    /// </summary>
    public Vector2 GetRandomPosition()
    {
        Vector2 randomPos;
        int maxAttempts = 10;
        int attempts = 0;
        
        do
        {
            // Generate random position dalam radius
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float randomDistance = Random.Range(0f, radius);
            
            Vector2 offset = new Vector2(
                Mathf.Cos(randomAngle) * randomDistance,
                Mathf.Sin(randomAngle) * randomDistance
            );
            
            randomPos = (Vector2)transform.position + offset;
            attempts++;
            
            // Jika tidak check obstacle atau tidak ada obstacle, return
            if (!checkForObstacles || !HasObstacle(randomPos))
            {
                return randomPos;
            }
            
        } while (attempts < maxAttempts);
        
        // Fallback ke center jika tidak dapat posisi valid
        return transform.position;
    }
    
    /// <summary>
    /// Check apakah posisi ini ada obstacle
    /// </summary>
    private bool HasObstacle(Vector2 position)
    {
        Collider2D obstacle = Physics2D.OverlapCircle(position, 0.5f, obstacleLayer);
        return obstacle != null;
    }
    
    /// <summary>
    /// Check apakah zona aktif
    /// </summary>
    public bool IsActive()
    {
        return isActive;
    }
    
    /// <summary>
    /// Set zona aktif/nonaktif
    /// </summary>
    public void SetActive(bool active)
    {
        isActive = active;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = zoneColor;
        DrawCircle(transform.position, radius, 32);
    }
    
    private void OnDrawGizmos()
    {
        if (showInGame)
        {
            Gizmos.color = new Color(zoneColor.r, zoneColor.g, zoneColor.b, 0.3f);
            DrawCircle(transform.position, radius, 32);
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
}
