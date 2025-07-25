using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameSound : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance?.StopPlay("WakeUp");
        AudioManager.Instance?.Play("GameMusic");
    }
}
