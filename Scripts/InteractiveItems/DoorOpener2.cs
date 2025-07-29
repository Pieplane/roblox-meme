using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener2 : MonoBehaviour
{
    public Transform door;
    public Transform door2;
    public Vector3 openRotation = new Vector3(0, 90, 0);
    public Vector3 openRotation2 = new Vector3(0, -90, 0); // Пример: вторая дверь открывается в другую сторону
    public float openSpeed = 2f;
    public float closeDelay = 2f; // ⏱ задержка перед закрытием
    public GameObject textCanvas;

    private bool isOpen = false;
    private bool isMoving = false;
    private Quaternion closedRotation;
    private Quaternion closedRotation2;
    private Quaternion targetRotation;
    private Quaternion targetRotation2;
    private bool playerInside;
    private Coroutine closeDoorCoroutine; // 💡 для отмены задержки при возвращении игрока

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
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleDoor();
                VisibleText(false);
            }
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

            // ⛔ Отменяем закрытие, если игрок вернулся
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

            // ⏳ Запускаем закрытие двери с задержкой
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
        targetRotation = isOpen ? Quaternion.Euler(openRotation) * closedRotation : closedRotation;
        targetRotation2 = isOpen ? Quaternion.Euler(openRotation2) * closedRotation2 : closedRotation2;
        isMoving = true;
        VisibleText(false);

        //if (isOpen)
        //{
        //    AudioManager.Instance?.Play("Open");
        //}
        AudioManager.Instance?.Play("Open");
    }

    private void VisibleText(bool visible)
    {
        textCanvas.SetActive(visible);
    }
}
