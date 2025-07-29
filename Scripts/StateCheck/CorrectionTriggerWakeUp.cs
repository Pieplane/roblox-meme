using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectionTriggerWakeUp : MonoBehaviour
{
    [SerializeField] private string exeptionCheckpoint;
    [SerializeField] private GameObject wakeUpText;
    private string currentCheckpointID;
    private void OnEnable()
    {
        CheckpointManager.OnCheckpointsLoaded += HandleCheckpointsLoaded;
    }

    private void OnDisable()
    {
        CheckpointManager.OnCheckpointsLoaded -= HandleCheckpointsLoaded;
    }

    private void HandleCheckpointsLoaded()
    {
        currentCheckpointID = CheckpointManager.Instance.GetLastCheckpointID();
        //Debug.Log("Сейчас у нас: " + currentCheckpointID);
        if (currentCheckpointID == exeptionCheckpoint)
        {
            wakeUpText.SetActive(false);
        }
    }
}
