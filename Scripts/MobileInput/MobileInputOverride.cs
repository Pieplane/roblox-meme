using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MobileInputOverride : MonoBehaviour
{
    [Header("Ссылки")]
    public VariableJoystick joystick;
    public ThirdPersonController controller;

    [Header("Состояние управления")]
    public bool inputEnabled = true;

    private StarterAssetsInputs _input;

    private void Start()
    {
        _input = controller.GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        if (!inputEnabled) return;

        // 🛑 Если джойстик не активен — принудительно сбрасываем ввод
        if (!joystick.gameObject.activeInHierarchy)
        {
            _input.MoveInput(Vector2.zero);
            return;
        }

        Vector2 moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        _input.MoveInput(moveInput);
    }

    /// <summary>
    /// Включает или отключает управление (например, из Timeline).
    /// </summary>
    //public void SetInputEnabled(bool value)
    //{
    //    inputEnabled = value;

    //    // Скрыть или показать джойстик
    //    if (joystick != null)
    //    {
    //        Transform background = joystick.transform.GetChild(0);
    //        background.gameObject.SetActive(value);
    //    }


    //    // Обнулить ввод при отключении
    //    if (!value && _input != null)
    //    {
    //        _input.MoveInput(Vector2.zero);
    //        _input.JumpInput(false);
    //    }

    //    Debug.Log("Управление мобильное: " + (value ? "включено" : "отключено"));
    //}
    public void SetInputEnabled(bool value)
    {
        inputEnabled = value;

        // Скрыть или показать джойстик
        if (joystick != null)
        {
            Transform background = joystick.transform.GetChild(0);
            background.gameObject.SetActive(value);
        }

        // ВАЖНО: Обнулить ввод немедленно
        if (_input != null)
        {
            _input.MoveInput(Vector2.zero);   // 👈 Сброс движения
            _input.JumpInput(false);          // 👈 Сброс прыжка
        }

        //Debug.Log("Управление мобильное: " + (value ? "включено" : "отключено"));
    }
}
