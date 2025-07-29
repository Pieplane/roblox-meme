using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentLoaderHelp : MonoBehaviour
{
    public string checkpointID;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EnvironmentLoader.Instance?.LoadEnvironmentByCheckpoint(checkpointID);
        }
    }
}
