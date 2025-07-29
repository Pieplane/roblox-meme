using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeCheckPoint : MonoBehaviour
{
    public bool needAdv = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (needAdv)
            {
                AdController.Instance.TryShowAdFromCheckpoint(() =>
                {
                    Debug.Log("⛳ Реклама с чекпоинта завершена!");
                    // Можно добавить дополнительную логику после рекламы
                });
            }
        }
        
    }
}
