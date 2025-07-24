using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
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

    public float duration = 10f;
    private bool isMobile;

    private Coroutine currentCoroutine;
    [SerializeField] private CanvasGroup dialogueCanvasGroup;

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

    public void TogglePauseMenu()
    {
        bool isNowActive = !settingsMenu.activeSelf;
        settingsMenu.SetActive(isNowActive);
        homeButtonGameObject.SetActive(isNowActive);
        if (!isMobile)
        {
            controlsButtonGameObject.SetActive(isNowActive);
        }
        

        Time.timeScale = isNowActive ? 0f : 1f;

        if (GameManager.Instance != null)
        {
            // Если мы НЕ в диалоге — переключаем курсор
            if (GameManager.Instance.GetCurrentState() != GameState.Dialogue)
            {
                GameManager.Instance.SetCursor(isNowActive);
            }
            // Если В диалоге — оставляем курсор включённым
            else
            {
                GameManager.Instance.SetCursor(true);
            }
        }

        mobileInput?.SetInputEnabled(!isNowActive);
        mobileRotation?.SetRotationEnabled(!isNowActive);
        controller?.SetIsRotationEnabled(!isNowActive);

        bool isNowActiveControls = controlsMenu.activeSelf;
        if (isNowActiveControls)
        {
            ToggleControlsMenu();
        }
        // ⛔ Отключаем взаимодействие с диалогом, если открыты настройки
        if (dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.blocksRaycasts = !isNowActive;
        }
    }
    public void ToggleControlsMenu()
    {
        bool isNowActive = !controlsMenu.activeSelf;
        controlsMenu.SetActive(isNowActive);
        settingsMenu.SetActive(!isNowActive);
    }

    public void CloseSettings()
    {
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
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        GameManager.Instance.ButtonExit();
        //SceneManager.LoadScene("Menu");
    }
    public void TeleportToCheckpoint()
    {
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
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ActivateArrowsTemporarily(duration));
    }
}