using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectionTrigger : MonoBehaviour
{
    [SerializeField] private string exeptionCheckpoint;
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
        Debug.Log("Сейчас у нас: " + currentCheckpointID);
        if(currentCheckpointID == exeptionCheckpoint)
        {
            gameObject.SetActive(false);
        }
    }
}
