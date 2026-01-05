using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// How to Play Panel - Tutorial/Controls popup
/// </summary>
public class HowToPlayPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeButton;
    
    [Header("Content (Optional)")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;
    
    [Header("Multi-Page Support")]
    [SerializeField] private GameObject[] pages;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private TextMeshProUGUI pageIndicatorText;
    
    private int currentPageIndex = 0;
    
    void Awake()
    {
        // Setup button listeners di Awake
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
        
        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);
        
        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousPage);
        
        // CRITICAL: Hide panel IMMEDIATELY di Awake
        if (panel != null)
        {
            panel.SetActive(false);
            Debug.Log("[HowToPlayPanel] Panel hidden in Awake");
        }
    }
    
    void Start()
    {
        // Setup pages (optional, untuk multi-page)
        if (pages != null && pages.Length > 0)
        {
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] != null)
                {
                    pages[i].SetActive(i == 0);
                }
            }
        }
    }
    
    /// <summary>
    /// Show How to Play panel
    /// </summary>
    public void Show()
    {
        if (panel == null)
        {
            Debug.LogError("[HowToPlayPanel] Panel is NULL! Assign panel reference in Inspector.");
            return;
        }
        
        // Pastikan panel tidak sedang active
        if (panel.activeSelf)
        {
            Debug.LogWarning("[HowToPlayPanel] Panel already active, skipping...");
            return;
        }
        
        panel.SetActive(true);
        currentPageIndex = 0;
        ShowPage(currentPageIndex);
        
        // Play UI sound
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClick();
        }
        
        Debug.Log("[HowToPlayPanel] Panel shown");
    }
    
    /// <summary>
    /// Close How to Play panel
    /// </summary>
    public void Close()
    {
        if (panel != null && panel.activeSelf)
        {
            panel.SetActive(false);
            
            // Play UI sound
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlayButtonClick();
            }
            
            Debug.Log("[HowToPlayPanel] Panel closed");
        }
    }
    
    public void NextPage()
    {
        if (pages == null || pages.Length == 0) return;
        
        currentPageIndex++;
        if (currentPageIndex >= pages.Length)
        {
            currentPageIndex = pages.Length - 1;
        }
        
        ShowPage(currentPageIndex);
        
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClick();
        }
    }
    
    public void PreviousPage()
    {
        if (pages == null || pages.Length == 0) return;
        
        currentPageIndex--;
        if (currentPageIndex < 0)
        {
            currentPageIndex = 0;
        }
        
        ShowPage(currentPageIndex);
        
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClick();
        }
    }
    
    private void ShowPage(int pageIndex)
    {
        if (pages == null || pages.Length == 0) return;
        
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
            {
                pages[i].SetActive(i == pageIndex);
            }
        }
        
        if (previousButton != null)
        {
            previousButton.interactable = pageIndex > 0;
        }
        
        if (nextButton != null)
        {
            nextButton.interactable = pageIndex < pages.Length - 1;
        }
        
        if (pageIndicatorText != null)
        {
            pageIndicatorText.text = $"Page {pageIndex + 1}/{pages.Length}";
        }
    }
    
    public void SetContent(string title, string content)
    {
        if (titleText != null)
            titleText.text = title;
        
        if (contentText != null)
            contentText.text = content;
    }
}
