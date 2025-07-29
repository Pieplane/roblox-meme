using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MobileRotationOverride : MonoBehaviour
{
    public float lookSpeedTouch = 0.2f;
    public GameObject CinemachineCameraTarget;
    public ThirdPersonController controller;

    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private int touchFingerId = -1;

    private bool isActive = true; // 🔁 Можно управлять этим флагом извне

    [Range(0.1f, 10f)]
    public float CameraSensitivity = 1f;

    private void Start()
    {
        if (controller == null)
        {
            controller = FindObjectOfType<ThirdPersonController>();
        }

        _cinemachineTargetYaw = controller.transform.rotation.eulerAngles.y;

        CameraSensitivity = PlayerPrefs.GetFloat("CameraSensitivity", 1f);
    }

    private void LateUpdate()
    {
        if (!isActive) return; // 🚫 Управление отключено
        HandleTouchCamera();
        ApplyCameraRotation();
    }

    private void HandleTouchCamera()
    {
        if (Input.touchCount == 0)
        {
            touchFingerId = -1;
            return;
        }

        foreach (Touch touch in Input.touches)
        {
            if (touch.position.x < Screen.width / 2f)
                continue;

            if (touch.phase == TouchPhase.Began)
            {
                touchFingerId = touch.fingerId;
            }
            else if (touch.fingerId == touchFingerId && touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;

                _cinemachineTargetYaw -= delta.x * lookSpeedTouch * CameraSensitivity;
                _cinemachineTargetPitch += delta.y * lookSpeedTouch * CameraSensitivity;

                _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
            }
        }
    }

    private void ApplyCameraRotation()
    {
        if (CinemachineCameraTarget != null)
        {
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(
                _cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw,
                0.0f);
        }

        controller?.SetCameraRotation(_cinemachineTargetYaw, _cinemachineTargetPitch);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

    public void ResetCameraRotationMobile()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    // 🔧 Внешнее управление
    public void SetRotationEnabled(bool enabled)
    {
        isActive = enabled;
    }
}
