using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectionButtonTrigger : MonoBehaviour
{
    [SerializeField] private string exeption;

    private void Start()
    {
        HandleCheckpointsLoaded();
    }

    private void OnEnable()
    {
        CheckpointManager.OnCheckpointChanged += HandleCheckpointChanged;
        CheckpointManager.OnCheckpointsLoaded += HandleCheckpointsLoaded;
    }

    private void OnDisable()
    {
        CheckpointManager.OnCheckpointChanged -= HandleCheckpointChanged;
        CheckpointManager.OnCheckpointsLoaded -= HandleCheckpointsLoaded;
    }

    private void HandleCheckpointChanged(string checkpointID)
    {
        if (checkpointID == exeption)
        {
            gameObject.SetActive(false);
        }
    }

    public void HandleCheckpointsLoaded()
    {
        string currentCheckpointID = CheckpointManager.Instance.GetLastCheckpointID();
        Debug.Log("Сейчас у нас: " + currentCheckpointID);

        if (string.IsNullOrEmpty(currentCheckpointID) || currentCheckpointID == exeption)
        {
            if(gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            } 
        }
    }
}
