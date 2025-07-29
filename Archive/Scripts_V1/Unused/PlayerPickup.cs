using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPosition;     // �������, ��� ������������ ������
    public float moveSmoothness = 10f; // �������� ����������� �������
    public KeyCode interactKey = KeyCode.G; // ������ ��������������

    private GameObject pickedObject;    // ������� �������� ������
    private Collider objectInRange;     // ������, ��������� ��� ��������������
    private bool isHolding = false;     // ����: ������������ �� ������
    private float newRadius = 0.8f;
    private float currentRadius;
    CharacterController characterController;


    private void Start()
    {
        GameObject parent = transform.gameObject;

        // �������� �������� ��������� CharacterController
        if (parent.TryGetComponent<CharacterController>(out CharacterController controller))
        {
            characterController = controller;

            currentRadius = characterController.radius;
        }
        else
        {
            Debug.LogWarning("��������� CharacterController �� ������ �� �������.");
        }
    }

    void Update()
    {
        // ���� ����� �������� ������ ��������������
        if (Input.GetKeyDown(interactKey))
        {
            if (isHolding)
            {
                DropObject(); // ��������� ������
            }
            else if (objectInRange != null)
            {
                PickObject(objectInRange.gameObject); // ��������� ������
            }
        }

        // ���� ������ ������������, ���������� ��� ������ � �������
        if (isHolding && pickedObject != null)
        {
            MoveObjectWithPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ���������, ����� �� ������ ���� �������������� � �� ���������� �� ���-��
        if (other.CompareTag("Pickup") && !isHolding)
        {
            objectInRange = other;  // ��������� ������ �� ������ � ����
            Transform objectTransform = other.transform;  // �������� Transform �������
            if (objectTransform.childCount > 0)  // ���������, ���� �� �������� �������
            {
                Transform canvas = objectTransform.GetChild(0);  // ������ ������ �������� ������ ����������
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
        // ���� ������ �������� ����, ������� ��� �� ���������
        if (objectInRange == other)
        {
            objectInRange = null;
            Transform objectTransform = other.transform;  // �������� Transform �������
            if (objectTransform.childCount > 0)  // ���������, ���� �� �������� �������
            {
                Transform canvas = objectTransform.GetChild(0);  // ������ ������ �������� ������ ����������
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
        pickedObject = obj; // ��������� ������ �� ������
        isHolding = true;

        // ��������� ������ � �������
        if (pickedObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }

        // ���������� ������ � ������� ���������
        pickedObject.transform.position = holdPosition.position;
        pickedObject.transform.SetParent(holdPosition);
        Debug.Log($"������ {pickedObject.name} ��������.");

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
        // ��������� ������� ������������� �������
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
            // ���������� ������
            pickedObject.transform.SetParent(null);
            if (pickedObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.isKinematic = false; // �������� ������ �������
            }

            Debug.Log($"������ {pickedObject.name} ������.");
            pickedObject = null;
            isHolding = false;

            characterController.radius = currentRadius;

            
        }
    }
}
