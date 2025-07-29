using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private float delay = 1f;
    [SerializeField] private GameObject skipButton;
    private CorrectionButtonTrigger skipTrigger;

    private GameObject playerToRespawn;
    private GameObject enemyToRespawn;

    private string lastCheckpoint;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        skipTrigger = skipButton.GetComponent<CorrectionButtonTrigger>();
    }
    public void TriggerRespawn(GameObject player)
    {
        GameManager.Instance?.SetCursor(true);
        Debug.Log("Игрок упал, нужен респавн");
        AudioManager.Instance?.Play("Oh");

        playerToRespawn = player;

        // Взрыв
        if (explosion != null)
        {
            explosion.transform.position = player.transform.position;
            explosion.transform.rotation = player.transform.rotation;
            explosion.gameObject.SetActive(true);
        }

        // Отключаем игрока и управление
        var input = player.GetComponent<PlayerInput>();
        if (input != null) input.enabled = false;

        player.SetActive(false);

        // Показываем UI с кнопкой
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }
        YandexGameplayEvents.Instance?.OnGameplayStop();
        PauseMenuManager.Instance.IsRespawnPanelActive = true;
    }
    public void TriggerRespawnBarry(GameObject player, GameObject enemy)
    {
        GameManager.Instance?.SetCursor(true);
        Debug.Log("Игрок пойман, нужен респавн");

        playerToRespawn = player;
        enemyToRespawn = enemy;


        // Отключаем игрока и управление
        var input = player.GetComponent<PlayerInput>();
        if (input != null) input.enabled = false;

        player.SetActive(false);
        StartCoroutine(CallDiedPanel(1.5f));


        PauseMenuManager.Instance.IsRespawnPanelActive = true;
    }

    // Вызывается кнопкой "Возродиться"
    public void Respawn()
    {
        GameManager.Instance?.SetCursor(false);
        
        if (deathPanel != null) deathPanel.SetActive(false);
        
        if(enemyToRespawn != null)
        {
            //Debug.Log("Возрождаю врага");
            enemyToRespawn.GetComponent<EnemyAI>().SpeakReset();
            enemyToRespawn = null;
        }
        else
        {
            //Debug.Log("Нет врага для возрождения");
        }

        // 🔥 Показываем рекламу при возрождении
        AdController.Instance.ShowAdFromRespawn(() =>
        {
            StartCoroutine(CallWithDelay(playerToRespawn, delay));
        });
    }
    private IEnumerator CallDiedPanel(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Показываем UI с кнопкой
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }
        YandexGameplayEvents.Instance?.OnGameplayStop();
    }

    private IEnumerator CallWithDelay(GameObject player, float delay)
    {
        Debug.Log("Ожидаю респавн...");
        yield return new WaitForSeconds(delay);

        lastCheckpoint = CheckpointManager.Instance.GetLastCheckpointID();
        //EnvironmentLoader.Instance?.LoadEnvironmentByCheckpoint(lastCheckpoint);
        GameManager.Instance.EnterID(lastCheckpoint);
        player.SetActive(true);

        var input = player.GetComponent<PlayerInput>();
        if (input != null) input.enabled = true;

        var controller = player.GetComponent<ThirdPersonController>();
        if (controller != null) controller.ResetJumpBuffer();

        YandexGameplayEvents.Instance?.OnGameplayStart();
        PauseMenuManager.Instance.IsRespawnPanelActive = false;

        if (skipTrigger != null)
        {
            skipTrigger.HandleCheckpointsLoaded();
        }
    }
}
