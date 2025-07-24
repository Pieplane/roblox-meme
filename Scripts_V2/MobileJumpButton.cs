using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileJumpButton : MonoBehaviour
{
    public StarterAssetsInputs input; // ссылка на скрипт управления

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnJumpPressed);
    }

    private void OnJumpPressed()
    {
        if (input != null)
        {
            input.jump = true;
        }
    }
}
