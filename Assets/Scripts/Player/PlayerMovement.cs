using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// Atribut Movement
    [SerializeField] float kecepatan;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    [SerializeField] bool rotateTowardsMouse = true; // Toggle: true = rotate ke mouse, false = rotate ke WASD
    private Vector2 movement;
    private Vector2 screenBounds;
    private float playerHalfWidth;
    private float playerHalfHeight;

    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        playerHalfWidth = spriteRenderer.bounds.extents.x;
        playerHalfHeight = spriteRenderer.bounds.extents.y;
    }

    void Update()
    {
        HandleMovement();
        
        // Rotate player ke arah mouse (untuk shooting)
        if (rotateTowardsMouse)
        {
            RotateTowardsMouse();
        }
        
        ClampMovement();
    }

    private void HandleMovement()
    {
        // Cek apakah ada tombol arah yang ditekan
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");

        // Set parameter animator
        if (inputHorizontal != 0f || inputVertical != 0f)
        {
            animator.SetBool("isRunning", true);

            // Rotate karakter berdasarkan arah gerakan (jika tidak pakai mouse rotation)
            if (!rotateTowardsMouse)
            {
                RotateCharacter(inputHorizontal, inputVertical);
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("isHit");
        }

        // Movement horizontal dan vertikal dengan normalisasi untuk diagonal
        Vector2 inputVector = new Vector2(inputHorizontal, inputVertical);

        // Normalisasi untuk mencegah kecepatan lebih cepat saat diagonal
        if (inputVector.magnitude > 1f)
        {
            inputVector.Normalize();
        }

        movement = inputVector * kecepatan * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }
    
    private void RotateTowardsMouse()
    {
        // Dapatkan posisi mouse di world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Set z ke 0 untuk 2D
        
        // Hitung arah dari player ke mouse
        Vector2 direction = (mousePos - transform.position).normalized;
        
        // Hitung angle dan rotate player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void RotateCharacter(float horizontal, float vertical)
    {
        // Rotasi berdasarkan WASD (untuk mode non-mouse rotation)
        float angle = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ClampMovement()
    {
        // Clamp horizontal
        float clampedX = Mathf.Clamp(transform.position.x, -screenBounds.x + playerHalfWidth, screenBounds.x - playerHalfWidth);

        // Clamp vertical
        float clampedY = Mathf.Clamp(transform.position.y, -screenBounds.y + playerHalfHeight, screenBounds.y - playerHalfHeight);

        Vector2 pos = transform.position;
        pos.x = clampedX;
        pos.y = clampedY;
        transform.position = pos;
    }
}
