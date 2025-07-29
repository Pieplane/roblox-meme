using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum GameState
{
    Sleeping,
    WaitingToWake,
    WaikingUp,
    Playing,
    Dialogue
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene References")]
    [SerializeField] private GameObject player;

    private GameState currentState;
    private bool currentCursorState;
    private string lastID;

    [SerializeField] private GameObject playerCameraRoot;
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private ParticleSystem respawnEffect;
    [SerializeField] private MobileRotationOverride mobileCam;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private HashSet<string> triggeredCutcsenes = new HashSet<string>();

    [SerializeField] private GameObject mobileInputGO;

    private void Awake()
    {
        //Singelton
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            //SceneManager.sceneLoaded += OnSceneLoaded; // ✅ Подписка
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //private void OnDestroy()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}
    private void Start()
    {
        initialPosition = playerCameraRoot.transform.localPosition;
        initialRotation = playerCameraRoot.transform.localRotation;
        lastID = CheckpointManager.Instance.GetLastCheckpointID();
        Debug.Log($"I Will loaded by lastID: {lastID}");
        EnterID(lastID);
        SetCursor(false);
    }
    private void Update()
    {
        //if (currentState == GameState.WaitingToWake && Input.GetKeyDown(KeyCode.E))
        //{
        //    Debug.Log("Нажимаешь Е, готов проснуться");
        //    TransitionToState(GameState.WaikingUp);
        //}
        //Debug.Log($"State: {currentState}");
    }
    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    Debug.Log($"Сцена загружена: {scene.name}");

    //    if (scene.name != "Menu") // или проверь конкретные игровые сцены
    //    {
    //        lastID = CheckpointManager.Instance.GetLastCheckpointID();
    //        EnterID(lastID);
    //        SetCursor(false);
    //    }
    //}
    //public void WaikungUp()
    //{
    //    Debug.Log("Нажимаешь Е, готов проснуться");
    //    TransitionToState(GameState.WaikingUp);
    //}
    public void TransitionToState(GameState newState)
    {
        SetCursor(false);
        currentState = newState;
        //EnterState(currentState);
    }

    //public void ExitState()
    //{
    //    CutsceneManager.Instance.EndCutscene();
    //}
    private void StagePreparation(string checkpointID, GameObject player)
    {
        Debug.Log("Подготавливаю сцену");

        Transform spawnPoint = CheckpointManager.Instance.GetSpawnPoint(checkpointID);
        if (spawnPoint == null)
        {
            Debug.LogError($"Не найден spawnPoint для чекпоинта {checkpointID}");
            return;
        }
        respawnEffect.transform.position = spawnPoint.position;
        respawnEffect.gameObject.SetActive(true);

        // 1. Деактивируем CharacterController перед перемещением
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // 2. Активируем игрока (если вдруг выключен)
        if (!player.activeSelf)
        {
            player.SetActive(true);
        }

        // 3. Устанавливаем позицию и поворот игрока
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;


        // 4. Сбрасываем положение и поворот cameraRoot (если привязано к игроку)
        //Debug.Log($"Initial cameraRoot position: {initialPosition} Initial cameraRoot rotation: {initialRotation}");
        playerCameraRoot.transform.localPosition = initialPosition;
        playerCameraRoot.transform.localRotation = initialRotation;
        //Debug.Log($"Current cameraRoot position: {playerCameraRoot.transform.localPosition} Current cameraRoot rotation: {playerCameraRoot.transform.localRotation}");

        // 5. Сброс значений углов для CameraRotation
        // Вызов сброса углов в контроллере камеры
        player.GetComponent<ThirdPersonController>()?.ResetCameraRotation();
        mobileCam?.ResetCameraRotationMobile();

        // 6. Активируем CharacterController обратно
        if (cc != null) cc.enabled = true;

        // 8. Принудительная проверка: вернем прыжки, если игрок НЕ в NoJumpZone
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, 0.5f);
        bool inNoJumpZone = false;
        foreach (var col in colliders)
        {
            if (col.GetComponent<NoJumpZone>() != null)
            {
                inNoJumpZone = true;
                break;
            }
        }

        var controller = player.GetComponent<ThirdPersonController>();
        if (controller != null)
        {
            controller.canJump = !inNoJumpZone;
        }

        // 7. Включаем управление (если отключено)
        var input = player.GetComponent<PlayerInput>();
        if (input != null && !input.enabled)
        {
            input.enabled = true;
        }
        Debug.Log("Игрок успешно телепортирован на чекпоинт: " + checkpointID);

        EnsureMobileInput();
    }
    private void EnsureMobileInput()
    {
        if (!Application.isMobilePlatform)
            return;

        if (mobileInputGO != null && !mobileInputGO.activeSelf)
        {
            mobileInputGO.SetActive(true);
            Debug.Log("✅ MobileInput включён автоматически.");
        }
    }
    public void EnterID(string checkpointID)
    {
        switch (checkpointID)
        {
            case "WakeUp":
                Debug.Log("Просыпаюсь");
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("WakeUp");
                //Initial cutscene
                break;
            case "Started":
                Debug.Log("Уже проснулся, загружаю начальную комнату");
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("Cutcsene_Started");
                StagePreparation(checkpointID, player);
                //Initial cutscene
                break;
            case "Pipes":
                Debug.Log("Чекпоинт в комнате с трубами");
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("Cutcsene_Pipes");
                StagePreparation(checkpointID, player);
                //Pipes cutcsene
                break;
            case "Dialogue1":
                TransitionToState(GameState.Dialogue); // 👈 ЭТО ДОБАВЬ
                CutsceneManager.Instance.StartCutscene("Dialogue1");
                SetCursor(true);
                break;
            case "Dialogue2":
                TransitionToState(GameState.Dialogue); // 👈 ЭТО ДОБАВЬ
                CutsceneManager.Instance.StartCutscene("Dialogue2");
                SetCursor(true);
                break;
            case "Dialogue3":
                TransitionToState(GameState.Dialogue); // 👈 ЭТО ДОБАВЬ
                CutsceneManager.Instance.StartCutscene("Dialogue3");
                SetCursor(true);
                break;
            case "Dialogue4":
                TransitionToState(GameState.Dialogue); // 👈 ЭТО ДОБАВЬ
                CutsceneManager.Instance.StartCutscene("Dialogue4");
                SetCursor(true);
                break;
            case "Meet":
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("Corridor0");
                ResetTriggeredCutscenes();
                StagePreparation(checkpointID, player);
                break;
            case "Trace1":
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("Trace1");
                StagePreparation(checkpointID, player);
                break;
            case "Trace2":
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("Trace2");
                StagePreparation(checkpointID, player);
                break;
            case "Trace3":
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("Trace3");
                StagePreparation(checkpointID, player);
                break;
            case "Trace4":
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("Trace4");
                StagePreparation(checkpointID, player);
                break;
            case "Trace5":
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("Trace5");
                StagePreparation(checkpointID, player);
                break;
            case "Trace6":
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("Trace6");
                StagePreparation(checkpointID, player);
                break;
            case "Trace7":
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("Trace7");
                StagePreparation(checkpointID, player);
                break;
            case "Trace8":
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("Trace8");
                StagePreparation(checkpointID, player);
                break;
            case "Hall":
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("Hall");
                StagePreparation(checkpointID, player);
                break;
            case "Button":
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("Button");
                ResetTriggeredCutscenes();
                StagePreparation(checkpointID, player);
                break;
            case "Toilet":
                TransitionToState(GameState.Playing);
                CutsceneManager.Instance.StartCutscene("Toilet");
                StagePreparation(checkpointID, player);
                break;
            default:
                Debug.Log("Нет сохраненных чекпоинтов загружаю самое начало");
                TransitionToState(GameState.WaitingToWake);
                CutsceneManager.Instance.StartCutscene("Sleep");
                //TransitionToState(GameState.Sleeping);
                break;
        }
    }
    //private void EnterState(GameState state)
    //{
    //    Debug.Log("Current state: " + state);
    //    switch (state)
    //    {
    //        case GameState.Sleeping:
    //            //Действия когда спит
    //            CutsceneManager.Instance.StartCutscene("Sleep");
    //            break;
    //        case GameState.WaikingUp:
    //            //Действия когда просыпаемся
    //            CutsceneManager.Instance.StartCutscene("WakeUp");
    //            break;
    //        case GameState.Playing:
    //            //Действия когда игра
    //            break;
    //        case GameState.Dialogue:
    //            //CutsceneManager.Instance.StartCutscene("Dialogue1");
    //            SetCursor(true);
    //            break;
    //    }
    //}
    public void SetState(GameState newState)
    {
        Debug.Log("End cutcsene, state changed to: " + newState);
        currentState = newState;
    }
    public void SetCursor(bool cursorState)
    {
        currentCursorState = cursorState;
        if (currentCursorState == false)
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }
    public bool HasCutcseneBeenTriggered(string cutcseneName)
    {
        return triggeredCutcsenes.Contains(cutcseneName);
    }
    public void MarkCutcsenesAsTriggered(string cutcseneName)
    {
        if (!triggeredCutcsenes.Contains(cutcseneName))
        {
            triggeredCutcsenes.Add(cutcseneName);
        }
    }
    public void ResetTriggeredCutscenes()
    {
        triggeredCutcsenes.Clear();
        Debug.Log("Сброшены все флаги запущенных катсцен");
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Debug.Log("Потеря фокуса — ставлю игру на паузу");
            if (CutsceneManager.activeCutscene != null)
            {
                Time.timeScale = 0f; // ставим всё на паузу
            }
        }
        else
        {
            Debug.Log("Фокус вернулся — продолжаю игру");
            if (CutsceneManager.activeCutscene != null)
            {
                Time.timeScale = 1f; // снимаем паузу
            }

            if (currentState == GameState.Dialogue)
            {
                SetCursor(true); // возвращаем курсор
            }
        }
    }
    public void ExitToMainMenu()
    {
        Debug.Log("Выход в главное меню");

        // Сброс активных данных
        currentState = GameState.Sleeping;
        lastID = "Started";
        ResetTriggeredCutscenes();

        // Отключить управление, если надо
        if (player != null)
        {
            var input = player.GetComponent<PlayerInput>();
            if (input != null) input.enabled = false;

            var cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.SetActive(false);
        }
        SetCursor(true);
        //Destroy(gameObject);
        // Загружаем сцену главного меню
        SceneManager.LoadScene("Menu"); // убедись, что сцена добавлена в Build Settings
    }
    public void ButtonExit()
    {
        //Destroy(gameObject);
        // Загружаем сцену главного меню
        SceneManager.LoadScene("Menu"); // убедись, что сцена добавлена в Build Settings
    }
    public void TeleportToNextCheckpoint()
    {
        string currentID = CheckpointManager.Instance.GetLastCheckpointID();
        var orderedIDs = CheckpointManager.Instance.spawnPointList.Select(e => e.checkpointID).ToList();

        int currentIndex = orderedIDs.IndexOf(currentID);

        // Если игрок еще не достиг ни одного чекпоинта — начнем с первого
        if (currentIndex == -1)
        {
            if (orderedIDs.Count > 0)
            {
                string firstID = orderedIDs[0];
                Debug.Log("Нет достигнутых чекпоинтов, телепортирую на первый: " + firstID);
                EnterID(firstID);
            }
            else
            {
                Debug.LogWarning("Список чекпоинтов пуст!");
            }
            return;
        }

        int nextIndex = currentIndex + 1;
        if (nextIndex < orderedIDs.Count)
        {
            string nextID = orderedIDs[nextIndex];
            Debug.Log("Телепортирую на следующий чекпоинт: " + nextID);
            EnterID(nextID);
            CheckpointManager.Instance.SetCheckpoint(nextID); // опционально, если ты хочешь его сразу сохранить
        }
        else
        {
            Debug.Log("Последний чекпоинт, дальше некуда.");
        }
    }
    public GameState GetCurrentState()
    {
        return currentState;
    }
}
