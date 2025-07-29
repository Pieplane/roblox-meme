using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoubleJumpTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private Coroutine timerCoroutine;

    private void Start()
    {
        timerText.gameObject.SetActive(false); // скрыт в начале
    }

    public void StartDoubleJumpTimer(float duration)
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(TimerCoroutine(duration));
    }

    private IEnumerator TimerCoroutine(float duration)
    {
        timerText.gameObject.SetActive(true);
        float remainingTime = duration;

        while (remainingTime > 0f)
        {
            // Если открыт экран респавна — не тикаем
            if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsRespawnPanelActive)
            {
                yield return null;
                continue;
            }

            timerText.text = Mathf.CeilToInt(remainingTime).ToString();
            yield return null;
            remainingTime -= Time.deltaTime;
        }

        timerText.gameObject.SetActive(false);
    }
}
