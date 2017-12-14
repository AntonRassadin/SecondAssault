using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    //[SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private Slider mouseSensivitySlider;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    private void Start()
    {
        SetOptions();
    }

    private void SetOptions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("resolution", currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();
        mouseSensivitySlider.value = PlayerPrefs.GetFloat("mouse sensivity", 5);
        volumeSlider.value = PlayerPrefs.GetFloat("volume", .5f);
        qualityDropdown.value = PlayerPrefs.GetInt("quality", 2);
        fullscreenToggle.isOn = (PlayerPrefs.GetInt("fullscreen") == 1) ? true : false;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Cursor.lockState = CursorLockMode.None;
        PlayerPrefs.SetInt("resolution", resolutionIndex);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", (isFullscreen) ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("quality", qualityIndex);
        PlayerPrefs.Save();
    }

    public void SetVolue(float volume)
    {
        //audioMixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.Save();
        AudioManager.instance.SetVolume();
    }

    public void MainMenuOpen()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OptionsOpen()
    {
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public void SetMouseSensivity(float sensivity)
    {
        PlayerPrefs.SetFloat("mouse sensivity", sensivity);
        PlayerPrefs.Save();
    }
}
