using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    Resolution[] resolutions;
    public Dropdown resolutionDropdown;

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        Debug.Log(volume);
    }

    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; ++i)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullScreen (bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
