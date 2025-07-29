using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedLookController : MonoBehaviour
{
    [Header("Camera Rotation")]
    public Transform cameraPivot;
    public float lookSpeed = 0.2f;

    //private float yaw = 0f;
    //private float pitch = 0f;

    private bool isLying = true;

    //private int touchFingerId = -1;
    private Vector2 lastTouchPosition;
    public bool requireRightMouseHold = false; // Нужно ли зажимать ПКМ

    private void Update()
    {
        if (isLying)
        {
            //// если вращение не зависит от ПКМ или ПКМ зажата
            //if (!requireRightMouseHold || Input.GetMouseButton(1))
            //{
            //    RotateHeadWithMouse();
            //}

            if (Input.GetKeyDown(KeyCode.E))
            {
                
                GetUp();
            }
            //RotateHeadWithTouch();
        }
    }

    //private void ApplyRotation(float deltaYaw, float deltaPitch)
    //{
    //    yaw += deltaYaw;
    //    yaw = Mathf.Clamp(yaw, -60f, 60f);

    //    pitch -= deltaPitch;
    //    pitch = Mathf.Clamp(pitch, -60f, 60f);

    //    cameraPivot.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    //}

    //private void RotateHeadWithMouse()
    //{
    //    if (Input.GetMouseButton(1))
    //    {
    //        float mouseX = Input.GetAxis("Mouse X") * lookSpeed * 100f;
    //        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed * 100f;

    //        ApplyRotation(mouseX * Time.deltaTime, mouseY * Time.deltaTime);
    //    }
    //}

    //private void RotateHeadWithTouch()
    //{
    //    foreach (Touch touch in Input.touches)
    //    {
    //        if (touch.phase == TouchPhase.Began)
    //        {
    //            touchFingerId = touch.fingerId;
    //            lastTouchPosition = touch.position;
    //        }
    //        else if (touch.fingerId == touchFingerId && touch.phase == TouchPhase.Moved)
    //        {
    //            Vector2 delta = touch.position - lastTouchPosition;
    //            lastTouchPosition = touch.position;

    //            float deltaYaw = delta.x * lookSpeed * Time.deltaTime;
    //            float deltaPitch = delta.y * lookSpeed * Time.deltaTime;

    //            ApplyRotation(deltaYaw, deltaPitch);
    //        }
    //        else if (touch.fingerId == touchFingerId && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
    //        {
    //            touchFingerId = -1;
    //        }
    //    }
    //}

    public void GetUp()
    {
        AudioManager.Instance?.Play("Click");
        isLying = false;
        GameManager.Instance.EnterID("WakeUp");
        Debug.Log("GetUp");
    }
}
