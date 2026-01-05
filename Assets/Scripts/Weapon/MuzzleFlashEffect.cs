using UnityEngine;

public class MuzzleFlashEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField] private float lifetime = 0.05f; // Sangat cepat (50ms)
    [SerializeField] private bool randomRotation = true;
    [SerializeField] private bool fadeOut = true;
    
    [Header("Sprite Variations (Optional)")]
    [SerializeField] private Sprite[] spriteVariations; // Array of different flash sprites
    [SerializeField] private bool useRandomSprite = true;
    
    private SpriteRenderer spriteRenderer;
    private float spawnTime;
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            Debug.LogError("MuzzleFlashEffect: No SpriteRenderer found! Adding one...");
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }
    
    void Start()
    {
        spawnTime = Time.time;
        
        // Random sprite variation untuk visual diversity
        if (useRandomSprite && spriteVariations != null && spriteVariations.Length > 0)
        {
            spriteRenderer.sprite = spriteVariations[Random.Range(0, spriteVariations.Length)];
        }
        
        // Pastikan sprite ter-assign
        if (spriteRenderer.sprite == null)
        {
            Debug.LogWarning("MuzzleFlashEffect: No sprite assigned! Flash will be invisible.");
        }
        
        // Random rotation untuk variasi
        if (randomRotation)
        {
            transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        }
        
        Debug.Log($"MuzzleFlashEffect started on {gameObject.name}, lifetime: {lifetime}s");
    }
    
    void Update()
    {
        if (spriteRenderer == null) return;
        
        float elapsed = Time.time - spawnTime;
        
        // Fade out effect
        if (fadeOut)
        {
            float alpha = 1f - (elapsed / lifetime);
            Color color = spriteRenderer.color;
            color.a = Mathf.Clamp01(alpha);
            spriteRenderer.color = color;
        }
        
        // Destroy after lifetime
        if (elapsed >= lifetime)
        {
            Debug.Log($"MuzzleFlashEffect destroying {gameObject.name} after {elapsed}s");
            Destroy(gameObject);
        }
    }
}
