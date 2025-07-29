using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //[SerializeField] private ParticleSystem checkpointEffect;
    public string checkpointID;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 checkpointPos = transform.position;
            if (!CheckpointManager.Instance.IsCheckpointReached(checkpointID))
            {
                CheckpointManager.Instance.SetCheckpoint(checkpointID);
                //if (checkpointEffect != null)
                //{
                //    checkpointEffect.transform.position = other.transform.position;
                //    checkpointEffect.gameObject.SetActive(true);
                //}
            }
            
        }
    }
}
