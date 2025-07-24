using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedDoor : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance?.Play("Close");
            canvas.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canvas.SetActive(false);
        }
    }
}
