using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPosition;     // Позиция, где удерживается объект
    public float moveSmoothness = 10f; // Скорость перемещения объекта
    public KeyCode interactKey = KeyCode.G; // Кнопка взаимодействия

    private GameObject pickedObject;    // Текущий поднятый объект
    private Collider objectInRange;     // Объект, доступный для взаимодействия
    private bool isHolding = false;     // Флаг: удерживается ли объект
    private float newRadius = 0.8f;
    private float currentRadius;
    CharacterController characterController;


    private void Start()
    {
        GameObject parent = transform.gameObject;

        // Пытаемся получить компонент CharacterController
        if (parent.TryGetComponent<CharacterController>(out CharacterController controller))
        {
            characterController = controller;

            currentRadius = characterController.radius;
        }
        else
        {
            Debug.LogWarning("Компонент CharacterController не найден на объекте.");
        }
    }

    void Update()
    {
        // Если игрок нажимает кнопку взаимодействия
        if (Input.GetKeyDown(interactKey))
        {
            if (isHolding)
            {
                DropObject(); // Отпустить объект
            }
            else if (objectInRange != null)
            {
                PickObject(objectInRange.gameObject); // Подобрать объект
            }
        }

        // Если объект удерживается, перемещаем его вместе с игроком
        if (isHolding && pickedObject != null)
        {
            MoveObjectWithPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, имеет ли объект слой взаимодействия и не удерживаем ли что-то
        if (other.CompareTag("Pickup") && !isHolding)
        {
            objectInRange = other;  // Сохраняем ссылку на объект в зоне
            Transform objectTransform = other.transform;  // Получаем Transform объекта
            if (objectTransform.childCount > 0)  // Проверяем, есть ли дочерние объекты
            {
                Transform canvas = objectTransform.GetChild(0);  // Делаем первый дочерний объект неактивным
                if (canvas.childCount > 0)
                {
                    canvas.GetChild(0).gameObject.SetActive(true);
                    canvas.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Если объект покидает зону, убираем его из доступных
        if (objectInRange == other)
        {
            objectInRange = null;
            Transform objectTransform = other.transform;  // Получаем Transform объекта
            if (objectTransform.childCount > 0)  // Проверяем, есть ли дочерние объекты
            {
                Transform canvas = objectTransform.GetChild(0);  // Делаем первый дочерний объект неактивным
                if (canvas.childCount > 0)
                {
                    canvas.GetChild(0).gameObject.SetActive(false);
                    //canvas.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
    }

    void PickObject(GameObject obj)
    {
        pickedObject = obj; // Сохраняем ссылку на объект
        isHolding = true;

        // Отключаем физику у объекта
        if (pickedObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }

        // Перемещаем объект к позиции удержания
        pickedObject.transform.position = holdPosition.position;
        pickedObject.transform.SetParent(holdPosition);
        Debug.Log($"Объект {pickedObject.name} подобран.");

        characterController.radius = newRadius;
        if(pickedObject.transform.childCount > 0)
        {
            Transform canvas = pickedObject.transform.GetChild(0);
            if(canvas.childCount > 0)
            {
                canvas.GetChild(0).gameObject.SetActive(false);
                canvas.GetChild(1).gameObject.SetActive(true);
            }
        }
        
    }

    void MoveObjectWithPlayer()
    {
        // Обновляем позицию удерживаемого объекта
        Vector3 targetPosition = holdPosition.position;
        pickedObject.transform.position = Vector3.Lerp(pickedObject.transform.position, targetPosition, Time.deltaTime * moveSmoothness);
    }

    void DropObject()
    {
        if (pickedObject != null)
        {
            if (pickedObject.transform.childCount > 0)
            {
                Transform canvas = pickedObject.transform.GetChild(0);
                if (canvas.childCount > 0)
                {
                    canvas.GetChild(0).gameObject.SetActive(false);
                    canvas.GetChild(1).gameObject.SetActive(false);
                }
            }
            // Сбрасываем объект
            pickedObject.transform.SetParent(null);
            if (pickedObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.isKinematic = false; // Включаем физику обратно
            }

            Debug.Log($"Объект {pickedObject.name} опущен.");
            pickedObject = null;
            isHolding = false;

            characterController.radius = currentRadius;

            
        }
    }
}
