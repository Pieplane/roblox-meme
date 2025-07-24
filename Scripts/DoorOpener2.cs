using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener2 : MonoBehaviour
{
    public Transform door;
    public Transform door2;
    public Vector3 openRotation = new Vector3(0, 90, 0);
    public Vector3 openRotation2 = new Vector3(0, 90, 0);
    public float openSpeed = 2f;
    public GameObject textCanvas;
    public GameObject textCanvas2;

    private bool isOpen = false;
    private bool isMoving = false;
    private Quaternion closedRotation;
    private Quaternion closedRotation2;
    private Quaternion targetRotation;
    private Quaternion targetRotation2;
    private bool playerInside;




    private void Start()
    {
        closedRotation = door.rotation;
        closedRotation2 = door2.rotation;
        VisibleText(false);
    }
    private void Update()
    {
        if (playerInside)
        {

            //PC: key E
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleDoor();
                VisibleText(false);
            }
            //Mobile devices: touch on screen
            //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            //{
            //    ToggleDoor();
            //    VisibleText(false);
            //}
        }
        if (isMoving)
        {

            door.rotation = Quaternion.Lerp(door.rotation, targetRotation, Time.deltaTime * openSpeed);
            door2.rotation = Quaternion.Lerp(door2.rotation, targetRotation2, Time.deltaTime * openSpeed);
            if (Quaternion.Angle(door.rotation, targetRotation) < 0.1f && Quaternion.Angle(door2.rotation, targetRotation2) < 0.1f)
            {
                door.rotation = targetRotation;
                door2.rotation = targetRotation2;
                isMoving = false;
                if (playerInside)
                {
                    VisibleText(true);
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            VisibleText(true);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            VisibleText(false);
            if (isOpen)
            {
                ToggleDoor(); // Закрыть дверь, если она была открыта
            }
        }

    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        targetRotation = isOpen ? Quaternion.Euler(openRotation) * closedRotation : closedRotation;
        targetRotation2 = isOpen ? Quaternion.Euler(openRotation2) * closedRotation2 : closedRotation2;
        isMoving = true;
        VisibleText(false);
    }
    private void VisibleText(bool text)
    {
        if (textCanvas.activeSelf != text && textCanvas2.activeSelf != text)
        {
            textCanvas.SetActive(text);
            textCanvas2.SetActive(text);
        }

    }
}
