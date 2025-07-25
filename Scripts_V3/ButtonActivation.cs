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
        yield return new WaitForSeconds(.4f); // Подождать 1 кадр (или чуть больше)
        AudioManager.Instance?.Play("Open2");
    }
}
