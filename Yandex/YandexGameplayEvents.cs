using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class YandexGameplayEvents : MonoBehaviour
{
    public static YandexGameplayEvents Instance { get; private set; }

    [DllImport("__Internal")]
    private static extern void StartGameplay();

    [DllImport("__Internal")]
    private static extern void StopGameplay();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Предотвращаем дублирование
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject); // Если нужно сохранить при смене сцен
    }

    public void OnGameplayStart()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        StartGameplay();
        //Debug.Log("▶️ Yandex Gameplay Start");
#endif
    }

    public void OnGameplayStop()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        StopGameplay();
        //Debug.Log("⏹️ Yandex Gameplay Stop");
#endif
    }
}
