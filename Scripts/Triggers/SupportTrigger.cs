using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportTrigger : MonoBehaviour
{
    public void GotPosition()
    {
        GameManager.Instance.EnterID(CheckpointManager.Instance.GetLastCheckpointID());
    }
}
