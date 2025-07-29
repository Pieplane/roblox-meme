using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
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

    public bool IsMenuOpen { get; private set; }

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
        skipButton.onClick.AddListener(TeleportToCheckpoint);
        springButton.onClick.AddListener(OnButtonPressed);
    }

    private void Update()
    {
        if (!isMobile)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TogglePauseMenu();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TeleportToCheckpoint();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && controller != null)
            {
                //controller.EnableDoubleJump(10f);
                if (arrowSpawner != null)
                    StartCoroutine(ActivateArrowsTemporarily(duration));
            }
        }    
    }
    private void OnEnable()
    {
        ThirdPersonController.WasDoubleJump += HandleDoubleJump;
    }
    private void OnDisable()
    {
        ThirdPersonController.WasDoubleJump -= HandleDoubleJump;
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
    }

    public void ReturnToMainMenu()
    {
        AudioManager.Instance?.Play("Bub");
        Time.timeScale = 1f;

        AudioManager.Instance?.StopPlay("GameMusic");
        AudioManager.Instance?.Play("MenuMusic");
        GameManager.Instance.ButtonExit();
        //SceneManager.LoadScene("Menu");
    }
    public void TeleportToCheckpoint()
    {
        if (IsMenuOpen)
        {
            Debug.LogWarning("⛔ Нельзя телепортироваться во время открытого меню!");
            return;
        }

        AudioManager.Instance?.Play("Bub");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TeleportToNextCheckpoint();
        }
    }
    private IEnumerator ActivateArrowsTemporarily(float duration)
    {
        arrowSpawner.Activate();
        controller?.EnableDoubleJump();
        yield return new WaitForSeconds(duration);
        controller?.DisableDoubleJump();
        arrowSpawner.Deactivate();
    }
    public void OnButtonPressed()
    {
        if (IsMenuOpen)
        {
            Debug.LogWarning("⛔ Нельзя телепортироваться во время открытого меню!");
            return;
        }

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ActivateArrowsTemporarily(duration));
        AudioManager.Instance?.Play("Bub");
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
}