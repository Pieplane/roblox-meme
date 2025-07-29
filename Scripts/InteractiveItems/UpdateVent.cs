using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateVent : MonoBehaviour
{
    [SerializeField] private VentInteraction ventInteraction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(ventInteraction != null && ventInteraction.enabled == true)
            {
                ventInteraction.ResetVent();
                ventInteraction.enabled = false;
                Debug.Log("Деактивирую решеьку");
            }
            else
            {
                Debug.Log("Решетка уже деактивирована");
            }
        }
    }
}
