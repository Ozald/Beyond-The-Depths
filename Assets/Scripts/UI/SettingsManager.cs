using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class SettingsManager : MonoBehaviour
{
    public Text masterVolumePercent;
    public Text musicVolumePercent;
    public Text sfxVolumePercent;

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");

        if (PlayerPrefs.HasKey("MusicVolume"))
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");

        if (PlayerPrefs.HasKey("SFXVolume"))
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }
    void Update()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
            masterVolumePercent.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("MasterVolume") * 100) + "%";

        if (PlayerPrefs.HasKey("MusicVolume"))
            musicVolumePercent.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("MusicVolume") * 100) + "%";

        if (PlayerPrefs.HasKey("SFXVolume"))
            sfxVolumePercent.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("SFXVolume") * 100) + "%";
    }
}
