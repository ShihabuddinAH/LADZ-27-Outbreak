using UnityEngine;

public class AnimatedMuzzleFlash : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Sprite[] flashSprites; // Array of muzzle flash sprites
    [SerializeField] private float frameRate = 30f; // FPS untuk animation
    [SerializeField] private bool randomStartFrame = true;
    [SerializeField] private bool loop = false;
    
    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private float frameDuration;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (flashSprites == null || flashSprites.Length == 0)
        {
            Debug.LogWarning("AnimatedMuzzleFlash: No sprites assigned!");
            Destroy(gameObject);
            return;
        }
        
        frameDuration = 1f / frameRate;
        
        // Random start frame untuk variasi
        if (randomStartFrame)
        {
            currentFrame = Random.Range(0, flashSprites.Length);
        }
        
        // Random rotation
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        
        // Set initial sprite
        UpdateSprite();
    }
    
    void Update()
    {
        frameTimer += Time.deltaTime;
        
        if (frameTimer >= frameDuration)
        {
            frameTimer = 0f;
            currentFrame++;
            
            if (currentFrame >= flashSprites.Length)
            {
                if (loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    // Animation selesai, destroy
                    Destroy(gameObject);
                    return;
                }
            }
            
            UpdateSprite();
        }
    }
    
    private void UpdateSprite()
    {
        if (spriteRenderer != null && currentFrame < flashSprites.Length)
        {
            spriteRenderer.sprite = flashSprites[currentFrame];
        }
    }
}
