using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoJumpZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<ThirdPersonController>();
            if (player != null)
            {
                player.canJump = false;
            }
            Debug.Log("Зашел в зону без прыжков");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<ThirdPersonController>();
            if (player != null)
            {
                player.canJump = true;
            }
            Debug.Log("Вышел из зоны без прыжков");
        }
    }
}

