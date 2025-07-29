using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LoadingScene");
    }
    public void EndGame()
    {
        CheckpointManager.Instance.ResetCheckpoints();
        GameManager.Instance.ExitToMainMenu();
    }

}
