using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentLoader : MonoBehaviour
{
    public static EnvironmentLoader Instance { get; private set; }

    private List<string> loadedScenes = new List<string>();

    public static event Action OnEnvironmentLoaded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // на всякий случай
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        string checkpointID = CheckpointManager.Instance.GetLastCheckpointID();
        //Debug.Log($"[EnvironmentLoader] Последний чекпоинт: {checkpointID}");
        StartCoroutine(LoadEnvironmentForCheckpoint(checkpointID));
    }

    public void LoadEnvironmentByCheckpoint(string checkpointID)
    {
        //Debug.Log($"[EnvironmentLoader] Загрузка окружения для чекпоинта: {checkpointID}");
        StartCoroutine(LoadEnvironmentForCheckpoint(checkpointID));
    }

    private IEnumerator LoadEnvironmentForCheckpoint(string checkpointID)
    {
        List<string> scenesToLoad = GetScenesByCheckpoint(checkpointID);

        //Debug.Log($"[EnvironmentLoader] Сцены для загрузки: {string.Join(", ", scenesToLoad)}");
        //Debug.Log($"[EnvironmentLoader] Уже загруженные сцены: {string.Join(", ", loadedScenes)}");

        // Выгружаем сцены, которые больше не нужны
        foreach (string scene in loadedScenes)
        {
            if (!scenesToLoad.Contains(scene))
            {
                if (SceneManager.GetSceneByName(scene).isLoaded)
                {
                    yield return SceneManager.UnloadSceneAsync(scene);
                    //Debug.Log($"[EnvironmentLoader] Выгружена сцена: {scene}");
                }
            }
        }

        // Обновляем список (удаляем те, которые выгрузили)
        loadedScenes.RemoveAll(scene => !scenesToLoad.Contains(scene));

        // Загружаем недостающие сцены
        foreach (string sceneName in scenesToLoad)
        {
            if (!loadedScenes.Contains(sceneName))
            {
                if (!SceneManager.GetSceneByName(sceneName).isLoaded)
                {
                    yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    //Debug.Log($"[EnvironmentLoader] Загружена сцена: {sceneName}");
                }
                loadedScenes.Add(sceneName);
            }
        }

        //Debug.Log("[EnvironmentLoader] Загрузка окружения завершена.");
        // Вызовем событие
        OnEnvironmentLoaded?.Invoke();
    }

    private List<string> GetScenesByCheckpoint(string checkpointID)
    {
        //Debug.Log($"[EnvironmentLoader] Получаю сцены для чекпоинта: {checkpointID}");

        switch (checkpointID)
        {
            case "Started":
                return new List<string> { "PipesMeet" };
            case "Pipes":
                return new List<string> { "PipesMeet", "FirstMeet" };
            case "Meet":
                return new List<string> {"FirstMeet", "ToiletMeet" };
            case "Toilet":
                return new List<string> { "ToiletMeet", "ContentMeet"};
            case "Trace1":
            case "Trace2":
            case "Trace3":
            case "Trace4":
            case "Trace5":
            case "Trace6":
            case "Trace7":
            case "Trace8":
                return new List<string> { "ContentMeet", "EndMeet" };
            case "Hall":
            case "Button":
                return new List<string> { "EndMeet" };
            default:
                //Debug.LogWarning($"[EnvironmentLoader] Неизвестный чекпоинт: {checkpointID}");
                return new List<string>();
        }
    }
}
