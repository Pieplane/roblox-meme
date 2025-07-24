using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActivation : MonoBehaviour
{
    private void OnEnable()
    {
        AudioManager.Instance?.Play("Click");
        StartCoroutine(WaitOpen2Sound());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private IEnumerator WaitOpen2Sound()
    {
        yield return new WaitForSeconds(.4f); // ��������� 1 ���� (��� ���� ������)
        AudioManager.Instance?.Play("Open2");
    }
}
