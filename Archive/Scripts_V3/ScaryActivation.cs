using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaryActivation : MonoBehaviour
{
    [SerializeField] private string scarySound;
    private void OnEnable()
    {
        AudioManager.Instance?.StopPlay("MeetBarry");
        AudioManager.Instance?.Play(scarySound);
    }
    private void OnDisable()
    {
        AudioManager.Instance?.StopPlay(scarySound);
    }
}
