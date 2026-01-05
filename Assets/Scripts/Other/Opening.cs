using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Opening : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Nama scene MainMenu
    [SerializeField] private float minimumDisplayTime = 1f; // Waktu minimum logo ditampilkan (dalam detik)

    [Header("UI Elements (Optional)")]
    [SerializeField] private TextMeshProUGUI tapToStartText; // Text "Tap to Start" (opsional)
    [SerializeField] private float textBlinkSpeed = 1f; // Kecepatan kedip text

    [Header("Fade Settings (Optional)")]
    [SerializeField] private bool useFadeOut = true; // Gunakan fade out sebelum pindah scene
    [SerializeField] private float fadeOutDuration = 0.5f; // Durasi fade out
    [SerializeField] private CanvasGroup fadeCanvasGroup; // Canvas Group untuk fade effect

    private bool canProceed = false; // Flag untuk cek apakah sudah bisa proceed
    private bool isTransitioning = false; // Flag untuk cek apakah sedang transisi

    void Start()
    {
        // Setup fade jika digunakan
        if (useFadeOut && fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f; // Mulai transparan
        }

        // Mulai coroutine untuk menunggu minimum display time
        StartCoroutine(WaitForMinimumTime());

        // Mulai blink text jika ada
        if (tapToStartText != null)
        {
            StartCoroutine(BlinkText());
        }
    }

    void Update()
    {
        // Cek apakah user sudah bisa proceed dan belum sedang transisi
        if (canProceed && !isTransitioning)
        {
            // Deteksi input dari berbagai sumber
            if (DetectInput())
            {
                StartCoroutine(LoadMainMenu());
            }
        }
    }

    private bool DetectInput()
    {
        // Mouse click (untuk PC/Editor)
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }

        // Touch input (untuk mobile)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            return true;
        }

        // Keyboard input (spasi atau enter)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            return true;
        }

        // Any key (opsional, uncomment jika ingin)
        // if (Input.anyKeyDown)
        // {
        //     return true;
        // }

        return false;
    }

    private IEnumerator WaitForMinimumTime()
    {
        // Tunggu minimum display time
        yield return new WaitForSeconds(minimumDisplayTime);

        // Setelah waktu minimum terlewati, user boleh proceed
        canProceed = true;

        // Tampilkan text "Tap to Start" jika ada
        if (tapToStartText != null)
        {
            tapToStartText.gameObject.SetActive(true);
        }
    }

    private IEnumerator BlinkText()
    {
        // Sembunyikan text dulu jika belum bisa proceed
        if (tapToStartText != null && !canProceed)
        {
            tapToStartText.gameObject.SetActive(false);
        }

        while (true)
        {
            if (tapToStartText != null && canProceed && !isTransitioning)
            {
                // Blink effect menggunakan alpha
                float alpha = Mathf.PingPong(Time.time * textBlinkSpeed, 1f);
                Color color = tapToStartText.color;
                color.a = alpha;
                tapToStartText.color = color;
            }
            yield return null;
        }
    }

    private IEnumerator LoadMainMenu()
    {
        // Set flag sedang transisi
        isTransitioning = true;

        // Sembunyikan text "Tap to Start" saat mulai transisi
        if (tapToStartText != null)
        {
            tapToStartText.gameObject.SetActive(false);
        }

        // Fade out jika digunakan
        if (useFadeOut && fadeCanvasGroup != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeOutDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }

        // Load scene MainMenu
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
