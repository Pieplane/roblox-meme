using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MobileInputOverride : MonoBehaviour
{
    [Header("������")]
    public VariableJoystick joystick;
    public ThirdPersonController controller;

    [Header("��������� ����������")]
    public bool inputEnabled = true;

    private StarterAssetsInputs _input;

    private void Start()
    {
        _input = controller.GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        if (!inputEnabled) return;

        Vector2 moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        _input.MoveInput(moveInput);
    }

    /// <summary>
    /// �������� ��� ��������� ���������� (��������, �� Timeline).
    /// </summary>
    public void SetInputEnabled(bool value)
    {
        inputEnabled = value;

        // ������ ��� �������� ��������
        if (joystick != null)
        {
            Transform background = joystick.transform.GetChild(0);
            background.gameObject.SetActive(value);
        }
            

        // �������� ���� ��� ����������
        if (!value && _input != null)
        {
            _input.MoveInput(Vector2.zero);
            _input.JumpInput(false);
        }

        Debug.Log("���������� ���������: " + (value ? "��������" : "���������"));
    }
}
