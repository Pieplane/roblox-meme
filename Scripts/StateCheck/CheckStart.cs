using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckStart : MonoBehaviour
{
    [SerializeField] private string extentionSkip;

    private void Start()
    {
        //CheckpointManager.Instance.LoadCheckpoints();
        if (ShouldDisableSkip())
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
    private bool ShouldDisableSkip()
    {
        string currentCheckpoint = CheckpointManager.Instance.GetLastCheckpointID();
        return string.IsNullOrEmpty(currentCheckpoint) || currentCheckpoint == extentionSkip;
    }
}
