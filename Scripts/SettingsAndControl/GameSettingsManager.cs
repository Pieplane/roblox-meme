using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{
    public Slider sensitivitySlider;

    [SerializeField] private ThirdPersonController pcController;
    [SerializeField] private MobileRotationOverride mobileController;
    //private ThirdPersonController pcController;
    //private MobileRotationOverride mobileController;

    private void Start()
    {
        float savedSensitivity = PlayerPrefs.GetFloat("CameraSensitivity", 1f);
        sensitivitySlider.value = savedSensitivity;

        ApplySensitivity(savedSensitivity);

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("CameraSensitivity", value);
        PlayerPrefs.Save();

        ApplySensitivity(value);
    }

    private void ApplySensitivity(float value)
    {
        if (pcController != null)
            pcController.CameraSensitivity = value;

        if (mobileController != null)
            mobileController.CameraSensitivity = value;
    }
}
