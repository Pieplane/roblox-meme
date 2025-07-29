using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class SpawnPointEntry
{
    public string checkpointID;
    public Transform spawnTransform;
}
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    public List<SpawnPointEntry> spawnPointList;
    private Dictionary<string, Transform> spawnPointsDict = new Dictionary<string, Transform>();
    // Используем строковые ключи для избежания проблем с точностью Vector3
    public static Dictionary<string, int> checkpointData = new Dictionary<string, int>();

    private int currentCheckpointOrder = 1;

    public static event Action OnCheckpointsLoaded;
    public static event Action<string> OnCheckpointChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var entry in spawnPointList)
        {
            if (!spawnPointsDict.ContainsKey(entry.checkpointID)&& entry.spawnTransform != null)
            {
                spawnPointsDict.Add(entry.checkpointID, entry.spawnTransform);
            }
        }
        LoadCheckpoints();
    }

    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.Save();
        //LoadCheckpoints();

        foreach (var pair in checkpointData)
        {
            //Debug.Log($"Чекпоинт: {pair.Key}, Активен: {pair.Value}");
        }
        Debug.Log($"Max checkpoint: {GetLastCheckpointID()}");
    }
    public Transform GetSpawnPoint(string id)
    {
        if(spawnPointsDict.TryGetValue(id, out Transform point))
        {
            return point;
        }
        Debug.LogWarning($"Spawn point not found for ID: {id}");
        return null;
    }

    public void SetCheckpoint(string checkpointID)
    {
        if (!checkpointData.ContainsKey(checkpointID) || checkpointData[checkpointID]==0)
        {
            checkpointData[checkpointID] = currentCheckpointOrder++;
            SaveCheckpoints();
        }
        else
        {
            Debug.Log($"Чекпоинт {checkpointID} уже достигнут раньше");
        }
        

        //Debug.Log($"Достигнута позиция: {checkpointID} с порядком {checkpointData[checkpointID]}");
        SaveCheckpoints();

        OnCheckpointChanged?.Invoke(checkpointID); // 🔔 вызываем событие
    }
    public string GetLastCheckpointID()
    {
        if (checkpointData.Count == 0) return "";

        var maxEntry = checkpointData
            .Where(pair => pair.Value > 0)
            .OrderByDescending(pair => pair.Value)
            .FirstOrDefault();

        return maxEntry.Key ?? "";
    }
    public bool IsCheckpointReached(string id)
    {
        return checkpointData.ContainsKey(id);
    }

    private void SaveCheckpoints()
    {
        List<string> allKeys = new List<string>();

        foreach (var pair in checkpointData)
        {
            PlayerPrefs.SetInt("Checkpoint_" + pair.Key, pair.Value);
            allKeys.Add(pair.Key);
        }

        string allCheckpoints = string.Join(",", allKeys);
        PlayerPrefs.SetString("AllCheckpoints", allCheckpoints);
        //Debug.Log($"Список сохраненных чекпоинтов: {allCheckpoints}");
    }

    public void LoadCheckpoints()
    {
        checkpointData.Clear();

        string saved = PlayerPrefs.GetString("AllCheckpoints", "");

        if (string.IsNullOrEmpty(saved))
        {
            Debug.Log("Нет сохраненных чекпоинтов");
            return;
        }

        string[] ids = saved.Split(',');
        foreach (string id in ids)
        {
            int order = PlayerPrefs.GetInt("Checkpoint_" + id, 0);
            checkpointData[id] = order;

            if(order >= currentCheckpointOrder) 
            {
                currentCheckpointOrder = order + 1;
            }
        }

        Debug.Log("Все чекпоинты загружены");
        OnCheckpointsLoaded?.Invoke(); // 🔔 Вызов события
    }
    public void ResetCheckpoints()
    {
        checkpointData.Clear();
        currentCheckpointOrder = 1;

        // Удаляем из PlayerPrefs
        string saved = PlayerPrefs.GetString("AllCheckpoints", "");
        if (!string.IsNullOrEmpty(saved))
        {
            string[] ids = saved.Split(',');
            foreach (string id in ids)
            {
                PlayerPrefs.DeleteKey("Checkpoint_" + id);
            }
        }

        PlayerPrefs.DeleteKey("AllCheckpoints");
        PlayerPrefs.Save();

        Debug.Log("Чекпоинты полностью сброшены");
    }
    public bool HasSavedCheckpoints()
    {
        return PlayerPrefs.HasKey("AllCheckpoints");
    }
}