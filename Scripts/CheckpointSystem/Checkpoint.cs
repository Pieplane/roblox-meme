using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //[SerializeField] private ParticleSystem checkpointEffect;
    public string checkpointID;
    public bool needAdv = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetCheckpoint();
        }
    }
    public void GetCheckpoint()
    {
        //Vector3 checkpointPos = transform.position;
        // Показываем рекламу, если прошло ≥ 60 секунд
        if (needAdv)
        {
            AdController.Instance.TryShowAdFromCheckpoint(() =>
            {
                Debug.Log("⛳ Реклама с чекпоинта завершена!");
                // Можно добавить дополнительную логику после рекламы
            });
        }

        if (!CheckpointManager.Instance.IsCheckpointReached(checkpointID))
        {
            EnvironmentLoader.Instance?.LoadEnvironmentByCheckpoint(checkpointID);
            CheckpointManager.Instance.SetCheckpoint(checkpointID);
        }
    }
}
