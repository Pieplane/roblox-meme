using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public Dialogue dialogue;
    public Dialogue endDialogue;

    public float delayDialog;

    private bool inRange;
    private bool wasFirstDialogue = false;
    //private Dialogue currentDialogue;
    //private bool isWaitingToStart = false;

    public GameObject dialogueCanvas;
    public string nameDialogue;


    private void Start()
    {
        //currentDialogue = dialogue;
        dialogueCanvas.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterNPC(this, nameDialogue);
            Debug.Log("NPC зарегистрирован: " + nameDialogue);
        }
        else
        {
            Debug.LogWarning("GameManager.Instance ещё не готов, NPC не зарегистрирован: " + nameDialogue);
        }
    }

    private void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            // ⛔ Не позволяем активировать диалог, если он уже в состоянии диалога
            if (GameManager.Instance.GetCurrentState() == GameState.Dialogue)
                return;

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
            DialogueManager.Instance.StartDialogue(dialogue);
            wasFirstDialogue = true;
        }
        else
        {
            DialogueManager.Instance.StartDialogue(endDialogue);
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
