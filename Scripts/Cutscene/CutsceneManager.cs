using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;
    [SerializeField] private List<CutsceneStruct> cutscenes = new List<CutsceneStruct>();

    public static Dictionary<string, GameObject> cutsceneDataBase = new Dictionary<string, GameObject>();
    public static GameObject activeCutscene;

    private Action onCutsceneEnd; // ВАЖНО: будет вызвана после EndCutscene()

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeCutsceneDataBase();

        foreach (var cutscene in cutsceneDataBase)
        {
            cutscene.Value.SetActive(false);
        }
    }

    private void InitializeCutsceneDataBase()
    {
        cutsceneDataBase.Clear();

        foreach (var item in cutscenes)
        {
            cutsceneDataBase.Add(item.cutsceneKey, item.cutsceneObject);
        }
    }

    public void StartCutscene(string cutsceneKey)
    {
        StartCutscene(cutsceneKey, null); // вызываем перегруженную версию без onEnd
    }

    public void StartCutscene(string cutsceneKey, Action onEnd)
    {
        if (!cutsceneDataBase.ContainsKey(cutsceneKey))
        {
            Debug.LogError($"Катсцены c ключом \"{cutsceneKey}\" нет в базе");
            return;
        }

        if (activeCutscene == cutsceneDataBase[cutsceneKey])
        {
            EndCutscene(); // перезапуск
        }

        onCutsceneEnd = onEnd; // сохраняем коллбек

        activeCutscene = cutsceneDataBase[cutsceneKey];

        foreach (var cutscene in cutsceneDataBase)
        {
            cutscene.Value.SetActive(false);
        }

        cutsceneDataBase[cutsceneKey].SetActive(true);
    }

    public void EndCutscene()
    {
        if (activeCutscene != null)
        {
            activeCutscene.SetActive(false);
            activeCutscene = null;

            onCutsceneEnd?.Invoke(); // ВАЖНО: вызываем действие после завершения
            onCutsceneEnd = null;
        }
    }
}

// Структура катсцен для листа, чтобы потом присваивать эти значения к Key и Value в Dictionary cutsceneDataBase
[System.Serializable]
public struct CutsceneStruct
{
    public string cutsceneKey;
    public GameObject cutsceneObject;
}