using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialogueUI;
    public TMP_Text npcText;
    public Transform choicesContainer;
    public GameObject choiceButtonPrefab;
    public float typingSpeed = 0.05f;
    public bool IsDialogueActive { get; private set; } = false;

    private DialogueNode currentNode;
    private List<GameObject> choiceButtonsPool = new List<GameObject>();
    private Coroutine typingCoroutine;

    private TimelineController activeTimeline;

    private bool isTyping = false;

    [SerializeField] private GameObject skipButton;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Попытка запустить диалог");

        if (IsDialogueActive)
        {
            Debug.Log("Диалог уже активен, игнорирую");
            return;
        }

        //GameManager.Instance.TransitionToState(GameState.Dialogue);
        IsDialogueActive = true;
        dialogueUI.SetActive(true);
        ShowNode(dialogue.startNode);
    }

    private void ShowNode(DialogueNode node)
    {
        currentNode = node;
        npcText.text = node.npcText;

        // Сброс выбранного объекта (важно для Input System)
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

        // Отключаем все кнопки
        foreach (var btn in choiceButtonsPool)
        {
            btn.SetActive(false);
            btn.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        npcText.text = "";
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeTextAndShowChoices(node));
    }
    IEnumerator TypeTextAndShowChoices(DialogueNode node)
    {
        isTyping = true;
        skipButton?.SetActive(true); // 👈 показать кнопку

        npcText.text = "";

        foreach (char letter in node.npcText)
        {
            npcText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        skipButton?.SetActive(false); // 👈 скрыть кнопку

        // Показываем выборы только после окончания печати
        for (int i = 0; i < node.playerChoices.Count; i++)
        {
            GameObject buttonObj;

            if (i < choiceButtonsPool.Count)
            {
                buttonObj = choiceButtonsPool[i];
                buttonObj.SetActive(true);
            }
            else
            {
                buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
                choiceButtonsPool.Add(buttonObj);
            }

            var choice = node.playerChoices[i];
            buttonObj.GetComponentInChildren<TMP_Text>().text = choice.choiceText;

            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!isTyping) // защищаем от клика во время печати
                    OnChoiceSelected(choice);
            });
        }
    }

    void OnChoiceSelected(PlayerChoice choice)
    {
        //Debug.Log("Выбран ответ: " + choice.choiceText);
        if (choice.nextNode != null)
            ShowNode(choice.nextNode);
        else
            EndDialogue();
    }

    public void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;
        IsDialogueActive = false;
        dialogueUI.SetActive(false);
        GameManager.Instance.TransitionToState(GameState.Playing);
        activeTimeline?.ResumeTimeline();
    }
    public void SetActiveTimeline(TimelineController controller)
    {
        activeTimeline = controller;
    }
    public void SkipDialogueTyping()
    {
        if (isTyping && currentNode != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            npcText.text = currentNode.npcText;
            isTyping = false;
            skipButton?.SetActive(false); // ⛔ скрыть кнопку
            // Принудительно показать выбор
            StartCoroutine(ShowChoicesAfterSkip(currentNode));
        }
    }

    IEnumerator ShowChoicesAfterSkip(DialogueNode node)
    {
        yield return null; // ждём 1 кадр
        for (int i = 0; i < node.playerChoices.Count; i++)
        {
            GameObject buttonObj;

            if (i < choiceButtonsPool.Count)
            {
                buttonObj = choiceButtonsPool[i];
                buttonObj.SetActive(true);
            }
            else
            {
                buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
                choiceButtonsPool.Add(buttonObj);
            }

            var choice = node.playerChoices[i];
            buttonObj.GetComponentInChildren<TMP_Text>().text = choice.choiceText;

            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!isTyping)
                    OnChoiceSelected(choice);
            });
        }
    }
}
