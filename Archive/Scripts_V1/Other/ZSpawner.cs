using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZSpawner : MonoBehaviour
{
    public GameObject[] zObjects;
    public float delayBetweenZ = 0.3f;
    public float loopDelay = 1f;

    private void Start()
    {
        StartCoroutine(PlayZloop());
    }
    IEnumerator PlayZloop()
    {
        while (true)
        {
            //Включаем Z по очереди
            for (int i = 0; i < zObjects.Length; i++)
            {
                zObjects[i].SetActive(true);
                yield return new WaitForSeconds(delayBetweenZ);
            }
        }
    }
}

