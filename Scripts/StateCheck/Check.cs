using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check : MonoBehaviour
{
    private string currentCheckPoint;
    [SerializeField] private string exeption;
    private void Start()
    {
        currentCheckPoint = CheckpointManager.Instance.GetLastCheckpointID();

        if (string.IsNullOrEmpty(currentCheckPoint) || currentCheckPoint == exeption)
        {
            gameObject.SetActive(false);
            Debug.Log("Сейчас у нас: " + currentCheckPoint);
        }
        else
        {
            gameObject.SetActive(true);
            Debug.Log("НЕТ у нас: " + currentCheckPoint);
        }
    }
}
