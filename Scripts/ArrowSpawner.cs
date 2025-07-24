using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public GameObject[] arrowParents; // �������� � BillboardYOnly � ��������� �� �����
    public float delayBetweenArrows = 0.3f;
    public float loopDelay = 1f;

    private Coroutine spawnRoutine;

    public void Activate()
    {
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(PlayArrowLoop());
    }

    public void Deactivate()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        foreach (var arrow in arrowParents)
        {
            if (arrow != null)
                arrow.SetActive(false); // ��������� ��� �������
        }
    }

    private IEnumerator PlayArrowLoop()
    {
        while (true)
        {
            foreach (var arrow in arrowParents)
            {
                if (arrow != null)
                {
                    arrow.SetActive(false); // ����� (�� ������ ������)
                    arrow.SetActive(true);  // ��������� � ��������� Entry ��������
                    yield return new WaitForSeconds(delayBetweenArrows);
                }
            }

            yield return new WaitForSeconds(loopDelay);
        }
    }
}
