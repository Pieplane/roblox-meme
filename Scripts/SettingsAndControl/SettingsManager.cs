using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider sensitivitySlider;

    private void Start()
    {
        float savedSensitivity = PlayerPrefs.GetFloat("CameraSensitivity", 1f);
        sensitivitySlider.value = savedSensitivity;

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("CameraSensitivity", value);
        PlayerPrefs.Save();
    }
}
