using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private void Start()
    {
        // Загрузка сохранённых значений (по умолчанию -20 dB = ~0.1f, логарифмически)
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.75f);
        float sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 0.75f);

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        ApplyMusicVolume(musicVolume);
        ApplySFXVolume(sfxVolume);

        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
    }

    private void OnMusicSliderChanged(float value)
    {
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save();
    }

    private void OnSFXSliderChanged(float value)
    {
        ApplySFXVolume(value);
        PlayerPrefs.SetFloat(SFXVolumeKey, value);
        PlayerPrefs.Save();
    }

    private void ApplyMusicVolume(float value)
    {
        // dB (логарифмическое преобразование)
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }

    private void ApplySFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }
}
