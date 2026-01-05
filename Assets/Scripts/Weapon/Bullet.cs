using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private LayerMask hitLayers; // Layer yang bisa di-hit (Enemy, Wall, dll)
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject hitEffectPrefab; // Particle effect saat kena target
    [SerializeField] private TrailRenderer trailRenderer; // Trail effect (optional)
    
    private int damage;
    private Vector2 direction;
    private float spawnTime;
    private bool hasHit = false;
    private bool isInitialized = false; // Flag untuk cek sudah initialize
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D bulletCollider; // TAMBAHAN: Reference ke collider sendiri
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<Collider2D>(); // TAMBAHAN
        
        // Set sorting order lebih tinggi dari player
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 5;
        }
        
        // Pastikan Rigidbody2D tidak interfere dengan movement
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
            rb.gravityScale = 0f;
        }
    }
    
    private void OnEnable()
    {
        // Reset state saat bullet di-spawn
        hasHit = false;
        isInitialized = false;
        spawnTime = Time.time;
        
        // TAMBAHAN: Disable collider sebentar untuk avoid instant collision
        if (bulletCollider != null)
        {
            bulletCollider.enabled = false;
        }
    }
    
    private void Update()
    {
        // Jangan update jika belum initialized!
        if (!isInitialized || hasHit)
        {
            return;
        }
        
        // TAMBAHAN: Enable collider setelah bullet initialized dan bergerak sedikit
        // Delay lebih lama untuk safety (0.1s = 100ms)
        if (bulletCollider != null && !bulletCollider.enabled && Time.time > spawnTime + 0.1f)
        {
            bulletCollider.enabled = true;
        }
        
        // Gerakkan bullet
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        
        // Auto-destroy setelah lifetime habis
        if (Time.time >= spawnTime + lifetime)
        {
            DestroyBullet();
        }
    }
    
    private void FixedUpdate()
    {
        // Jangan collision check jika belum initialized!
        if (!isInitialized || hasHit)
        {
            return;
        }
        
        // Raycast untuk deteksi collision yang lebih akurat
        // PENTING: Start raycast SEDIKIT DI DEPAN bullet untuk avoid self-hit
        Vector2 rayStart = (Vector2)transform.position + (direction * 0.1f);
        
        RaycastHit2D hit = Physics2D.Raycast(
            rayStart, 
            direction, 
            speed * Time.fixedDeltaTime + 0.1f,
            hitLayers
        );
        
        if (hit.collider != null && hit.collider != bulletCollider) // PENTING: Ignore self
        {
            OnHit(hit);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Backup collision detection dengan trigger
        if (!isInitialized || hasHit)
        {
            return;
        }
        
        // PENTING: Ignore collision dengan diri sendiri!
        if (collision == bulletCollider)
        {
            return;
        }
        
        // TAMBAHAN: Ignore collision dengan weapon pickup
        if (collision.CompareTag("WeaponPickup") || collision.gameObject.layer == LayerMask.NameToLayer("Pickup") || collision.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
        {
            return;
        }
        
        // Cek apakah object ada di layer yang bisa di-hit
        if (((1 << collision.gameObject.layer) & hitLayers) != 0)
        {
            RaycastHit2D hit = new RaycastHit2D();
            OnHit(hit, collision);
        }
    }
    
    private void OnHit(RaycastHit2D hit, Collider2D fallbackCollider = null)
    {
        if (hasHit)
        {
            return;
        }
        
        hasHit = true;
        
        Collider2D targetCollider = hit.collider != null ? hit.collider : fallbackCollider;
        Vector2 hitPoint = hit.point != Vector2.zero ? hit.point : (Vector2)transform.position;
        
        if (targetCollider != null)
        {
            // Damage ke Enemy
            if (targetCollider.CompareTag("Enemy"))
            {
                ZombieHealth zombieHealth = targetCollider.GetComponent<ZombieHealth>();
                if (zombieHealth != null)
                {
                    zombieHealth.TakeDamage(damage);
                }
            }
            
            // Spawn hit effect
            if (hitEffectPrefab != null)
            {
                GameObject hitEffect = Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
                Destroy(hitEffect, 1f);
            }
        }
        
        // Destroy bullet
        DestroyBullet();
    }
    
    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
    
    // Method untuk setup bullet saat di-spawn
    public void Initialize(Vector2 shootDirection, int bulletDamage, LayerMask layers)
    {
        direction = shootDirection.normalized;
        damage = bulletDamage;
        hitLayers = layers;
        
        // Rotate bullet sprite menghadap arah tembakan
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // PENTING: Set initialized flag TERAKHIR
        isInitialized = true;
    }
    
    // TAMBAHAN: Overload Initialize dengan custom lifetime berdasarkan range
    public void Initialize(Vector2 shootDirection, int bulletDamage, LayerMask layers, float maxRange)
    {
        direction = shootDirection.normalized;
        damage = bulletDamage;
        hitLayers = layers;
        
        // Calculate lifetime berdasarkan range
        // lifetime = range / speed
        if (maxRange > 0 && speed > 0)
        {
            lifetime = maxRange / speed;
        }
        
        // Rotate bullet sprite menghadap arah tembakan
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // Set initialized flag
        isInitialized = true;
    }
}
