using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using StarterAssets;
using UnityEngine.InputSystem.XR;

public class AdController : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ShowAdv();

    public static AdController Instance;

    [SerializeField] private CanvasGroup countdownUI;
    [SerializeField] private TextMeshProUGUI countdownText;

    [SerializeField] private float minAdIntervalRespawn = 60f;
    [SerializeField] private float minAdIntervalCheckpoint = 120f;

    [SerializeField] private ThirdPersonController controller;
    [SerializeField] private MobileInputOverride mobileInput;
    [SerializeField] private MobileRotationOverride mobileRotation;

    private float lastAdTime = -999f; // чтобы первая могла сразу сработать

    private Action onAdCallback;
    private bool isAdRunning = false;

    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Используй при возрождении — показывает рекламу, если прошло ≥ 60 сек
    /// </summary>
    public void ShowAdFromRespawn(Action onAdFinished)
    {
        float timeSinceLastAd = Time.realtimeSinceStartup - lastAdTime;

        if (timeSinceLastAd >= minAdIntervalRespawn)
        {
            Debug.Log("📺 Реклама по возрождению");
            lastAdTime = Time.realtimeSinceStartup;
            ShowMyAd(onAdFinished);
        }
        else
        {
            Debug.Log($"⏱ Реклама при возрождении не показана, прошло только {timeSinceLastAd:F1} сек");
            onAdFinished?.Invoke(); // продолжаем без рекламы
        }
    }
    public void FakeRespawn(Action onAdFinished)
    {
        float timeSinceLastAd = Time.realtimeSinceStartup - lastAdTime;

        if (timeSinceLastAd >= minAdIntervalRespawn)
        {
            lastAdTime = Time.realtimeSinceStartup;
        }
        //else
        //{
        //    Debug.Log($"⏱ Реклама при возрождении не показана, прошло только {timeSinceLastAd:F1} сек");
        //}
    }

    /// <summary>
    /// Используй при входе в чекпоинт — запускает рекламу с отсчетом, если прошло ≥ 60 сек
    /// </summary>
    public void TryShowAdFromCheckpoint(Action onAdFinished)
    {
        float timeSinceLastAd = Time.realtimeSinceStartup - lastAdTime;

        if (timeSinceLastAd >= minAdIntervalCheckpoint)
        {
            Debug.Log("📺 Реклама с чекпоинта (с отсчетом)");
            StartCoroutine(ShowAdWithCountdown(onAdFinished));
        }
        else
        {
            Debug.Log($"⏱ Реклама с чекпоинта не показана, прошло только {timeSinceLastAd:F1} сек");
        }
    }

    private IEnumerator ShowAdWithCountdown(Action onAdFinished)
    {
        Time.timeScale = 0f;
        if(PauseMenuManager.Instance.IsRespawnPanelActive == false)
        {
            PauseMenuManager.Instance.IsRespawnPanelActive = true;
        }
        if(!PauseMenuManager.Instance.IsRewardnPanelActive)
        {
            PauseMenuManager.Instance.IsRewardnPanelActive = true;
        }
        //PauseMenuManager.Instance.IsRespawnPanelActive = true;

        countdownUI.gameObject.SetActive(true); // ← Включаем объект
        controller?.SetIsRotationEnabled(false);
        mobileInput?.SetInputEnabled(false);
        mobileRotation?.SetRotationEnabled(false);

        countdownUI.alpha = 1f;
        countdownUI.blocksRaycasts = true;

        for (int i = 2; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        countdownUI.alpha = 0f;
        countdownUI.blocksRaycasts = false;
        countdownUI.gameObject.SetActive(false); // ← Скрываем после отсчёта

        lastAdTime = Time.realtimeSinceStartup;

        ShowMyAd(() =>
        {
            Time.timeScale = 1f;
            onAdFinished?.Invoke();
        });
    }

    /// <summary>
    /// Здесь вызывается твой собственный метод показа рекламы
    /// </summary>
    private void ShowMyAd(Action onAdFinished)
    {
        if (isAdRunning)
        {
            Debug.LogWarning("⚠️ Реклама уже запущена!");
            return;
        }

        if (PauseMenuManager.Instance.IsRespawnPanelActive == false)
        {
            PauseMenuManager.Instance.IsRespawnPanelActive = true;
        }
        if (PauseMenuManager.Instance.IsRewardnPanelActive == false)
        {
            PauseMenuManager.Instance.IsRewardnPanelActive = true;
        }

        Debug.Log("🎥 Показываю Яндекс-рекламу через JS");
        onAdCallback = onAdFinished;
        isAdRunning = true;

#if UNITY_WEBGL && !UNITY_EDITOR
        ShowAdv(); // вызов реальной рекламы через JS
#else
        Debug.Log("🧪 [Editor] Симуляция рекламы...");
        StartCoroutine(FakeAdRoutine());
#endif
    }

    private IEnumerator FakeAdRoutine()
    {
        yield return new WaitForSecondsRealtime(2f);
        OnAdClosed(); // как будто реклама закрыта
    }
    public void OnAdClosed()
    {
        if (!isAdRunning) return;

        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
        isAdRunning = false;
        //Debug.Log("✅ Реклама завершена (через JS)");

        //PauseMenuManager.Instance.IsRespawnPanelActive = false;
        if (PauseMenuManager.Instance.IsRespawnPanelActive == true)
        {
            PauseMenuManager.Instance.IsRespawnPanelActive = false;
        }
        if (!PauseMenuManager.Instance.IsMenuOpen)
        {
            GameManager.Instance.SetCursor(false);
        }
        if (PauseMenuManager.Instance.IsRewardnPanelActive == true)
        {
            PauseMenuManager.Instance.IsRewardnPanelActive = false;
        }

        //GameManager.Instance.SetCursor(false);

        controller?.SetIsRotationEnabled(true);
        mobileInput?.SetInputEnabled(true);
        mobileRotation?.SetRotationEnabled(true);

        onAdCallback?.Invoke();
        onAdCallback = null;
    }
}
