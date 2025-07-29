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
            Destroy(gameObject); // �� ������ ������
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        string checkpointID = CheckpointManager.Instance.GetLastCheckpointID();
        //Debug.Log($"[EnvironmentLoader] ��������� ��������: {checkpointID}");
        StartCoroutine(LoadEnvironmentForCheckpoint(checkpointID));
    }

    public void LoadEnvironmentByCheckpoint(string checkpointID)
    {
        //Debug.Log($"[EnvironmentLoader] �������� ��������� ��� ���������: {checkpointID}");
        StartCoroutine(LoadEnvironmentForCheckpoint(checkpointID));
    }

    private IEnumerator LoadEnvironmentForCheckpoint(string checkpointID)
    {
        List<string> scenesToLoad = GetScenesByCheckpoint(checkpointID);

        //Debug.Log($"[EnvironmentLoader] ����� ��� ��������: {string.Join(", ", scenesToLoad)}");
        //Debug.Log($"[EnvironmentLoader] ��� ����������� �����: {string.Join(", ", loadedScenes)}");

        // ��������� �����, ������� ������ �� �����
        foreach (string scene in loadedScenes)
        {
            if (!scenesToLoad.Contains(scene))
            {
                if (SceneManager.GetSceneByName(scene).isLoaded)
                {
                    yield return SceneManager.UnloadSceneAsync(scene);
                    //Debug.Log($"[EnvironmentLoader] ��������� �����: {scene}");
                }
            }
        }

        // ��������� ������ (������� ��, ������� ���������)
        loadedScenes.RemoveAll(scene => !scenesToLoad.Contains(scene));

        // ��������� ����������� �����
        foreach (string sceneName in scenesToLoad)
        {
            if (!loadedScenes.Contains(sceneName))
            {
                if (!SceneManager.GetSceneByName(sceneName).isLoaded)
                {
                    yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    //Debug.Log($"[EnvironmentLoader] ��������� �����: {sceneName}");
                }
                loadedScenes.Add(sceneName);
            }
        }

        //Debug.Log("[EnvironmentLoader] �������� ��������� ���������.");
        // ������� �������
        OnEnvironmentLoaded?.Invoke();
    }

    private List<string> GetScenesByCheckpoint(string checkpointID)
    {
        //Debug.Log($"[EnvironmentLoader] ������� ����� ��� ���������: {checkpointID}");

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
                //Debug.LogWarning($"[EnvironmentLoader] ����������� ��������: {checkpointID}");
                return new List<string>();
        }
    }
}
