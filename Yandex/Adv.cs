using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adv : MonoBehaviour
{
    public static Adv Instance;
    //FULLSCREEN-------------------


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnOpen()
    {
        AudioListener.volume = 0;
        Debug.Log("AudioListener = 0");
    }

    public void OnClose()
    {
        AudioListener.volume = 1;
        Debug.Log("🎧 Звук включён");

        // Сообщаем AdController'у, что реклама завершена
        if (AdController.Instance != null)
            AdController.Instance.OnAdClosed();
    }

    public void OnError()
    {
        OnClose();
    }

    public void OnOffline()
    {
        OnClose();
    }

    //REWARD-----------------------

    public void OnOpenReward()
    {
        AudioListener.volume = 0;
    }

    public void OnRewarded(string rewardType)
    {
        Debug.Log($"🏆 Получена награда: {rewardType}");

        switch (rewardType)
        {
            case "teleport":
                Teleport(); // вызываем телепорт
                break;
            case "doublejump":
                EnableDoubleJump();
                break;
            default:
                Debug.LogWarning("❓ Неизвестный тип награды");
                break;
        }
    }

    public void OnCloseReward()
    {
        Debug.Log("Closse rewarded");
        AudioListener.volume = 1;
        PauseMenuManager.Instance?.OnAdClosedRewardAAA();
    }

    public void OnErrorReward()
    {
        OnCloseReward();
    }
    private void Teleport()
    {
        Debug.Log("🚀 Телепорт награждён!");
        // тут твой код телепорта
        PauseMenuManager.Instance?.TeleportToCheckpoint();
    }

    private void EnableDoubleJump()
    {
        Debug.Log("🦘 Двойной прыжок награждён!");
        // тут код для активации двойного прыжка
        PauseMenuManager.Instance?.OnButtonPressed();
    }
}
