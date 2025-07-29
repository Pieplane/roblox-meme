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
    }

    private void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogueNPC();
        }
    }
    public void StartDialogueNPC()
    {
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
    //IEnumerator DelayedStart(Dialogue dialogue)
    //{
    //    yield return new WaitForSeconds(delayDialog);
    //    FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    //    isWaitingToStart = false;
    //}
}
