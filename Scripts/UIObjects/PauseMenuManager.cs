using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ShowReward(string rewardType);

    public static PauseMenuManager Instance { get; private set; }

    public GameObject player;

    public Button homeButton;
    public Button optionButton;
    public Button closeOptionButton;
    public Button closeControlsButton;
    public Button controlButton;
    public Button skipButton;
    public Button springButton;

    public GameObject settingsMenu;
    public GameObject controlsMenu;

    public GameObject homeButtonGameObject;
    public GameObject controlsButtonGameObject;

    public MobileInputOverride mobileInput;
    public MobileRotationOverride mobileRotation;
    public ThirdPersonController controller;
    [SerializeField] private ArrowSpawner arrowSpawner;

    public float duration = 30f;
    private bool isMobile;

    private Coroutine currentCoroutine;
    [SerializeField] private CanvasGroup dialogueCanvasGroup;
    [SerializeField] private CanvasGroup skipCanvasGroup;
    [SerializeField] private CanvasGroup springCanvasGroup;
    [SerializeField] private DoubleJumpTimerUI timer;

    public bool IsMenuOpen { get; private set; }

    private bool canSkip;
    [SerializeField] private string extensionSkip;

    public bool IsRespawnPanelActive { get; set; }
    private float remainingDoubleJumpTime;

    public bool IsRewardnPanelActive { get; set; }
    public bool IsRewardnPanelActiveAfterJump { get; set; }

    private void Awake()
    {
        //Singelton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        var controller = player.GetComponent<ThirdPersonController>();

        isMobile = Application.isMobilePlatform;
        homeButton.onClick.AddListener(ReturnToMainMenu);
        optionButton.onClick.AddListener(TogglePauseMenu);
        closeOptionButton.onClick.AddListener(CloseSettings);
        closeControlsButton.onClick.AddListener(CloseSettings);
        controlButton.onClick.AddListener(ToggleControlsMenu);
        skipButton.onClick.AddListener(CallTeleportToCheckpoint);
        springButton.onClick.AddListener(CallDoubleJump);
    }

    private void Update()
    {
        if (!isMobile)
        {
            // ❗ Не реагировать на ввод, если открыт респавн UI
            if (IsRespawnPanelActive) return;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TogglePauseMenu();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (ShouldDisableSkip()) return;
                CallTeleportToCheckpoint();
                //TeleportToCheckpoint();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && controller != null)
            {
                if(ShouldDisableDoubleJump()) return;
                //controller.EnableDoubleJump(10f);
                CallDoubleJump();
                //if (arrowSpawner != null)
                //    StartCoroutine(ActivateArrowsTemporarily(duration));
            }
        }    
    }
    private void OnEnable()
    {
        ThirdPersonController.WasDoubleJump += HandleDoubleJump;
        NPCInteraction.DialogOpened += HideUI;
        DialogueManager.DialogClosed += ShowUI;
    }
    private void OnDisable()
    {
        ThirdPersonController.WasDoubleJump -= HandleDoubleJump;
        NPCInteraction.DialogOpened -= HideUI;
        DialogueManager.DialogClosed -= ShowUI;
    }
    private void HideUI()
    {
        skipButton.gameObject.SetActive(false);
        springButton.gameObject.SetActive(false);
    }
    private void ShowUI()
    {
        skipButton.gameObject.SetActive(true);
        springButton.gameObject.SetActive(true);
    }

    public void TogglePauseMenu()
    {
        AudioManager.Instance?.Play("Bub");

        // Учитываем оба окна
        bool isNowActive = !settingsMenu.activeSelf && !controlsMenu.activeSelf;

        settingsMenu.SetActive(isNowActive);
        homeButtonGameObject.SetActive(isNowActive);
        if (!isMobile)
        {
            controlsButtonGameObject.SetActive(isNowActive);
        }

        Time.timeScale = isNowActive ? 0f : 1f;

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.GetCurrentState() != GameState.Dialogue)
            {
                GameManager.Instance.SetCursor(isNowActive);
            }
            else
            {
                GameManager.Instance.SetCursor(true);
            }
        }

        mobileInput?.SetInputEnabled(!isNowActive);
        mobileRotation?.SetRotationEnabled(!isNowActive);
        controller?.SetIsRotationEnabled(!isNowActive);

        // 🎯 Главное исправление — обрабатываем звук правильно
        HandleAudioOnPauseToggle(isNowActive);

        // Если вдруг controlsMenu был открыт — закрываем его
        if (controlsMenu.activeSelf)
        {
            controlsMenu.SetActive(false);
            Debug.Log("Открыл попал сюда");
        }

        // Отключаем взаимодействие с диалогом, если открыты настройки
        if (dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.blocksRaycasts = !isNowActive;
        }
        if (skipCanvasGroup != null)
        {
            skipCanvasGroup.blocksRaycasts = !isNowActive;
        }
        if (springCanvasGroup != null)
        {
            springCanvasGroup.blocksRaycasts = !isNowActive;
        }

        IsMenuOpen = isNowActive;

        if (isNowActive)
            YandexGameplayEvents.Instance?.OnGameplayStop();
        else
            YandexGameplayEvents.Instance?.OnGameplayStart();
    }
    public void ToggleControlsMenu()
    {
        if (AudioManager.Instance != null && !AudioManager.Instance.IsPlaying("Bub"))
        {
            AudioManager.Instance?.Play("Bub");
        }

        bool isNowActive = !controlsMenu.activeSelf;
        controlsMenu.SetActive(isNowActive);
        settingsMenu.SetActive(!isNowActive);
    }

    public void CloseSettings()
    {
        AudioManager.Instance?.Play("Bub");
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        homeButtonGameObject.SetActive(false);
        controlsButtonGameObject.SetActive(false);
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            // Если мы НЕ в диалоге — переключаем курсор
            if (GameManager.Instance.GetCurrentState() != GameState.Dialogue)
            {
                GameManager.Instance.SetCursor(false);
            }
            // Если В диалоге — оставляем курсор включённым
            else
            {
                GameManager.Instance.SetCursor(true);
            }
        }

        mobileInput?.SetInputEnabled(true);
        mobileRotation?.SetRotationEnabled(true);
        controller?.SetIsRotationEnabled(true);

        // ⛔ Отключаем взаимодействие с диалогом, если открыты настройки
        if (dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.blocksRaycasts = true;
        }
        if (skipCanvasGroup != null)
        {
            skipCanvasGroup.blocksRaycasts = true;
        }
        if (springCanvasGroup != null)
        {
            springCanvasGroup.blocksRaycasts = true;
        }
        HandleAudioOnPauseToggle(false);
        IsMenuOpen = false;

        YandexGameplayEvents.Instance?.OnGameplayStart();
    }

    public void ReturnToMainMenu()
    {
        AudioManager.Instance?.Play("Bub");
        Time.timeScale = 1f;

        AudioManager.Instance?.StopPlay("GameMusic");
        AudioManager.Instance?.Play("MenuMusic");
        GameManager.Instance.ButtonExit();

        YandexGameplayEvents.Instance?.OnGameplayStop();
        //SceneManager.LoadScene("Menu");
    }
    public void CallTeleportToCheckpoint()
    {
        if (IsMenuOpen)
        {
            Debug.LogWarning("⛔ Нельзя телепортироваться во время открытого меню!");
            return;
        }
        if (IsRespawnPanelActive == false)
        {
            IsRespawnPanelActive = true;
        }

        AudioManager.Instance?.Play("Bub");
        IsRewardnPanelActive = true;
        Time.timeScale = 0f;

#if UNITY_WEBGL && !UNITY_EDITOR
        GameManager.Instance.SetCursor(true);
        ShowReward("teleport"); 
#endif
    }
    public void CallDoubleJump()
    {
        if (IsMenuOpen)
        {
            Debug.LogWarning("⛔ Нельзя телепортироваться во время открытого меню!");
            return;
        }
        if (IsRespawnPanelActive == false)
        {
            IsRespawnPanelActive = true;
        }
        AudioManager.Instance?.Play("Bub");
        IsRewardnPanelActiveAfterJump = true;
        Time.timeScale = 0f;


#if UNITY_WEBGL && !UNITY_EDITOR
        GameManager.Instance.SetCursor(true);
        ShowReward("doublejump"); 
#endif
    }
    public void TeleportToCheckpoint()
    {
        
        if (GameManager.Instance != null)
        {
            Time.timeScale = 1f;
            IsRewardnPanelActive = false;
            if (!IsRespawnPanelActive)
            {
                GameManager.Instance.SetCursor(false);
            }
            if (IsRespawnPanelActive == true)
            {
                IsRespawnPanelActive = false;
            }
            GameManager.Instance.TeleportToNextCheckpoint();
        }
    }
    private IEnumerator ActivateArrowsTemporarily(float duration)
    {
        remainingDoubleJumpTime = duration;

        controller?.EnableDoubleJump(duration);
        arrowSpawner.Activate();

        while (remainingDoubleJumpTime > 0f)
        {
            // Если открыт UI смерти — жди, не уменьшая таймер
            if (!IsRespawnPanelActive)
            {
                remainingDoubleJumpTime -= Time.deltaTime;
            }

            yield return null;
        }

        controller?.DisableDoubleJump();
        arrowSpawner.Deactivate();
    }
    public void OnAdClosedRewardAAA()
    {
        //if (currentCoroutine != null)
        //    StopCoroutine(currentCoroutine);

        Time.timeScale = 1f;
        IsRewardnPanelActiveAfterJump = false;
        IsRewardnPanelActive = false;
        if (IsRespawnPanelActive == true)
        {
            IsRespawnPanelActive = false;
        }
        GameManager.Instance.SetCursor(false);


    }
    public void OnButtonPressed()
    {
        

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        Time.timeScale = 1f;
        IsRewardnPanelActiveAfterJump = false;
        if (!IsRespawnPanelActive)
        {
            GameManager.Instance.SetCursor(false);
        }
        if (IsRespawnPanelActive == true)
        {
            IsRespawnPanelActive = false;
        }

        currentCoroutine = StartCoroutine(ActivateArrowsTemporarily(duration));

        timer.StartDoubleJumpTimer(duration);
    }
    private void HandleAudioOnPauseToggle(bool isNowActive)
    {
        if (isNowActive)
        {
            AudioManager.Instance?.StopAllExceptBub();
        }
        else
        {
            AudioManager.Instance?.ResumePrevious();
        }
    }
    private void HandleDoubleJump()
    {
        AudioManager.Instance?.Play("Poof");
    }
    private bool ShouldDisableSkip()
    {
        string currentCheckpoint = CheckpointManager.Instance.GetLastCheckpointID();
        return string.IsNullOrEmpty(currentCheckpoint) || currentCheckpoint == extensionSkip || GameManager.Instance?.GetCurrentState() == GameState.Dialogue;
    }
    private bool ShouldDisableDoubleJump()
    {
        string currentCheckpoint = CheckpointManager.Instance.GetLastCheckpointID();
        return string.IsNullOrEmpty(currentCheckpoint) || GameManager.Instance?.GetCurrentState() == GameState.Dialogue;
    }
}