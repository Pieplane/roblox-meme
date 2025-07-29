using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public Transform door;
    public Vector3 openRotation = new Vector3(0, 90, 0);
    public float openSpeed = 2f;
    public float closeDelay = 2f; // ⏱ задержка перед закрытием двери
    public GameObject textCanvas;
    //public GameObject textCanvas2;

    private bool isOpen = false;
    private bool isMoving = false;
    private Quaternion closedRotation;
    private Quaternion targetRotation;
    private bool playerInside;

    private Coroutine closeDoorCoroutine; // 💡 для отмены если игрок вернулся


    private void Start()
    {
        closedRotation = door.rotation;
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
            ////Mobile devices: touch on screen
            //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            //{
            //    ToggleDoor();
            //    VisibleText(false);
            //}
        }
        if (isMoving) 
        {
 
                door.rotation = Quaternion.Lerp(door.rotation, targetRotation, Time.deltaTime * openSpeed);
                if(Quaternion.Angle(door.rotation, targetRotation) < 0.1f)
                {
                    door.rotation = targetRotation;
                    isMoving = false;
                if(playerInside)
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

            // ⛔ Остановим корутину, если игрок снова зашёл
            if (closeDoorCoroutine != null)
            {
                StopCoroutine(closeDoorCoroutine);
                closeDoorCoroutine = null;
            }
        }
            
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            VisibleText(false);

            // ⏳ Запустить отложенное закрытие двери
            if (isOpen)
            {
                closeDoorCoroutine = StartCoroutine(CloseDoorWithDelay());
            }
        }
            
    }
    private IEnumerator CloseDoorWithDelay()
    {
        yield return new WaitForSeconds(closeDelay);
        if (!playerInside && isOpen)
        {
            ToggleDoor();
        }
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        targetRotation = isOpen ? Quaternion.Euler(openRotation)*closedRotation : closedRotation;
        isMoving = true;
        VisibleText(false);
        //if(isOpen == true)
        //{
        //    AudioManager.Instance?.Play("Open");
        //}
        AudioManager.Instance?.Play("Open");
    }
    private void VisibleText(bool text)
    {
        if (textCanvas.activeSelf != text)
        {
            textCanvas.SetActive(text);
        }
            
    }
}
