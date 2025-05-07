using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public AudioMixerGroup targetGroup;
    public SoundManager soundManager;
    public AudioMixer mixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundSlider;

    Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        // Get all available resolutions and populate the dropdown
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        soundManager = GameManager.instance.GetComponent<SoundManager>();
        masterSlider.value = Mathf.Pow(10, (PlayerPrefs.GetFloat("Master") / 20));
        soundSlider.value = Mathf.Pow(10, (PlayerPrefs.GetFloat("Sound") / 20));
        musicSlider.value = Mathf.Pow(10, (PlayerPrefs.GetFloat("Music") / 20));  
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        Debug.Log(qualityIndex);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log(isFullscreen);
    }

    public void SetResolution(int resolutionIndex)
    {
        Debug.Log("New resolution: " + resolutions[resolutionIndex]);
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetLevel(float sliderValue)
    {
        soundManager.SetVolume(targetGroup, sliderValue);
    }

    public void SetMasterLevel(float sliderValue)
    {
        soundManager.SetVolume(mixer.FindMatchingGroups("Master")[0], sliderValue);
    }

    public void SetSoundLevel(float sliderValue)
    {
        soundManager.SetVolume(mixer.FindMatchingGroups("Sound")[0], sliderValue);
    }
    public void SetMusicLevel(float sliderValue)
    {
        soundManager.SetVolume(mixer.FindMatchingGroups("Music")[0], sliderValue);
    }
    
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Master", masterSlider.value);
        PlayerPrefs.SetFloat("Sound", soundSlider.value);
        PlayerPrefs.SetFloat("Music", musicSlider.value);
        PlayerPrefs.Save();
    }
}
