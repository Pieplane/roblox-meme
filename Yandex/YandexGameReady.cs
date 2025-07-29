using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class YandexGameReady : MonoBehaviour
{
    private static bool alreadyReady = false;

    [DllImport("__Internal")]
    private static extern void GameReady();

    private void Start()
    {
        if (alreadyReady) return;

#if UNITY_WEBGL && !UNITY_EDITOR
        GameReady();
#endif
        alreadyReady = true;
    }
}
