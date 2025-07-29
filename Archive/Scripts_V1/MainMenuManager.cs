using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button continueButton;
    public Button newGameButton;
    public Button optionGameButton;
    public Button closeOptionGameButton;
    public Button controlsGameButton;
    public Button closeControlsGameButton;
    public GameObject settingsMenu;
    public GameObject controlsMenu;
    //public string gameSceneName = "New Scene"; // название сцены игры

    private void Start()
    {
        // Активируем кнопку "Продолжить", если есть сохранения
        if (PlayerPrefs.HasKey("AllCheckpoints"))
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }

        continueButton.onClick.AddListener(ContinueGame);
        newGameButton.onClick.AddListener(NewGame);
        optionGameButton.onClick.AddListener(OpenSettings);
        controlsGameButton.onClick.AddListener(OpenControls);
        closeOptionGameButton.onClick.AddListener(CloseSettings);
        closeControlsGameButton.onClick.AddListener(CloseControls);
    }

    void ContinueGame()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    void NewGame()
    {
        // Полный сброс сохранений
        PlayerPrefs.DeleteKey("AllCheckpoints");

        string saved = PlayerPrefs.GetString("AllCheckpoints", "");
        if (!string.IsNullOrEmpty(saved))
        {
            string[] ids = saved.Split(',');
            foreach (string id in ids)
            {
                PlayerPrefs.DeleteKey("Checkpoint_" + id);
            }
        }

        PlayerPrefs.Save();

        SceneManager.LoadScene("LoadingScene");
    }
    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        controlsMenu.SetActive(false);
    }
    public void OpenControls()
    {
        controlsMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
    }
    public void CloseControls()
    {
        controlsMenu.SetActive(false);
    }
}
