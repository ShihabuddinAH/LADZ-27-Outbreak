using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Settings Panel - Audio, Graphics, Controls settings.
/// </summary>
public class SettingsPanelUI : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    
    [Header("Graphics Settings")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    
    [Header("Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button resetButton;
    
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string FULLSCREEN_KEY = "Fullscreen";
    private const string QUALITY_KEY = "Quality";
    
    void Start()
    {
        // Setup button listeners
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
        
        if (applyButton != null)
            applyButton.onClick.AddListener(OnApplySettings);
        
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetSettings);
        
        // Setup slider listeners
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        
        // Setup toggle listener
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
        
        // Setup dropdown listeners
        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        
        // Load saved settings
        LoadSettings();
    }
    
    void OnEnable()
    {
        // Refresh settings when panel opens
        LoadSettings();
    }
    
    // ========== AUDIO SETTINGS ==========
    
    private void OnMusicVolumeChanged(float value)
    {
        if (musicVolumeText != null)
            musicVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        
        // Apply to audio mixer or AudioListener
        AudioListener.volume = value;
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        if (sfxVolumeText != null)
            sfxVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        
        // Apply to SFX audio source
        // TODO: Implement SFX volume control
    }
    
    // ========== GRAPHICS SETTINGS ==========
    
    private void OnFullscreenToggled(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    
    private void OnQualityChanged(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    
    // ========== SAVE/LOAD SETTINGS ==========
    
    private void OnApplySettings()
    {
        SaveSettings();
        Debug.Log("Settings applied!");
    }
    
    private void SaveSettings()
    {
        // Audio
        if (musicVolumeSlider != null)
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolumeSlider.value);
        
        if (sfxVolumeSlider != null)
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolumeSlider.value);
        
        // Graphics
        if (fullscreenToggle != null)
            PlayerPrefs.SetInt(FULLSCREEN_KEY, fullscreenToggle.isOn ? 1 : 0);
        
        if (qualityDropdown != null)
            PlayerPrefs.SetInt(QUALITY_KEY, qualityDropdown.value);
        
        PlayerPrefs.Save();
    }
    
    private void LoadSettings()
    {
        // Audio
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicVolume;
            OnMusicVolumeChanged(musicVolume);
        }
        
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = sfxVolume;
            OnSFXVolumeChanged(sfxVolume);
        }
        
        // Graphics
        bool fullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = fullscreen;
            Screen.fullScreen = fullscreen;
        }
        
        int quality = PlayerPrefs.GetInt(QUALITY_KEY, QualitySettings.GetQualityLevel());
        if (qualityDropdown != null)
        {
            qualityDropdown.value = quality;
            QualitySettings.SetQualityLevel(quality);
        }
    }
    
    private void OnResetSettings()
    {
        // Reset to defaults
        if (musicVolumeSlider != null) musicVolumeSlider.value = 1f;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = 1f;
        if (fullscreenToggle != null) fullscreenToggle.isOn = true;
        if (qualityDropdown != null) qualityDropdown.value = 2; // Medium quality
        
        SaveSettings();
        Debug.Log("Settings reset to default!");
    }
    
    private void OnBackButton()
    {
        // Return to main menu
        MainMenuManager menuManager = FindObjectOfType<MainMenuManager>();
        if (menuManager != null)
        {
            menuManager.ShowMainMenu();
        }
    }
}
