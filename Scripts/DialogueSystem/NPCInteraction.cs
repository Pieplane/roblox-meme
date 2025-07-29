using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalizedDialogue
{
    public string languageCode; // "en", "ru", "tr"
    public Dialogue dialogue;
    public Dialogue endDialogue;
}
public class NPCInteraction : MonoBehaviour
{
    [Header("Локализованные диалоги")]
    public List<LocalizedDialogue> localizedDialogues;

    //public Dialogue dialogue;
    //public Dialogue endDialogue;

    public float delayDialog;

    private bool inRange;
    private bool wasFirstDialogue = false;
    //private Dialogue currentDialogue;
    //private bool isWaitingToStart = false;

    public GameObject dialogueCanvas;
    public string nameDialogue;

    private Dialogue currentDialogue;
    private Dialogue currentEndDialogue;

    public static event Action DialogOpened;


    private void Start()
    {
        //currentDialogue = dialogue;
        dialogueCanvas.SetActive(false);

        // Установим текущие диалоги по языку
        SetCurrentDialogueByLanguage();

    }
    private void SetCurrentDialogueByLanguage()
    {
        string lang = Language.Instance?.CurrentLanguage ?? "en";
        //string lang = "en";

        foreach (var loc in localizedDialogues)
        {
            if (loc.languageCode == lang)
            {
                currentDialogue = loc.dialogue;
                currentEndDialogue = loc.endDialogue;
                return;
            }
        }

        // Если нет подходящего — используем английский
        var fallback = localizedDialogues.Find(l => l.languageCode == "en");
        if (fallback != null)
        {
            currentDialogue = fallback.dialogue;
            currentEndDialogue = fallback.endDialogue;
        }
    }

    private void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            // ⛔ Не позволяем активировать диалог, если он уже в состоянии диалога
            if (GameManager.Instance.GetCurrentState() == GameState.Dialogue)
                return;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterNPC(this, nameDialogue);
                Debug.Log("NPC зарегистрирован: " + nameDialogue);
            }
            else
            {
                Debug.LogWarning("GameManager.Instance ещё не готов, NPC не зарегистрирован: " + nameDialogue);
            }
            StartDialogueNPC();
        }
    }
    public void StartDialogueNPC()
    {
        //if (wasFirstDialogue)
        //    return; // ❌ Блокируем повторный запуск диалога

        Debug.Log("StartDialogue");
        // Если диалог уже идёт — не запускаем заново
        if (DialogueManager.Instance.IsDialogueActive)
            return;

        GameManager.Instance.EnterID(nameDialogue);
        //Debug.Log("Все хорошо передал гей менеджеру имя диалога");
        AudioManager.Instance?.Play("Answer");
        //StartCoroutine(DelayedStart(currentDialogue));
        //isWaitingToStart = true;
        //if (!wasFirstDialogue)
        //{
        //    wasFirstDialogue = true;
        //    currentDialogue = endDialogue;
        //}
        GameManager.Instance.CurrentNPCName = nameDialogue;
        OpenDialog();
    }
    public void OpenDialog()
    {
        Debug.Log("📖 Диалог открыт");
        DialogOpened?.Invoke();
        // Здесь может быть логика открытия окна
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
            dialogueCanvas.SetActive(true);
            Debug.Log("Player in range");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
            dialogueCanvas.SetActive(false);
        }
    }
    public void CutsceneStartDialogut()
    {
        if (!wasFirstDialogue)
        {
            DialogueManager.Instance.StartDialogue(currentDialogue);
            wasFirstDialogue = true;
        }
        else
        {
            DialogueManager.Instance.StartDialogue(currentEndDialogue);
        }
    }
    public void ForceExitRange()
    {
        inRange = false;
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);
    }
    //IEnumerator DelayedStart(Dialogue dialogue)
    //{
    //    yield return new WaitForSeconds(delayDialog);
    //    FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    //    isWaitingToStart = false;
    //}
}
