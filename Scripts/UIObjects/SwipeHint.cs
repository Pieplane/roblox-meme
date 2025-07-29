using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeHint : MonoBehaviour
{
    private bool hintHidden = false;
    //private int activeFingerId = -1;
    private Coroutine autoHideCoroutine;

    private void OnEnable()
    {
        // Запускаем таймер автоскрытия при активации объекта
        if (autoHideCoroutine != null)
            StopCoroutine(autoHideCoroutine);

        autoHideCoroutine = StartCoroutine(AutoHideHint(4f));
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    //private void Update()
    //{
    //    if (hintHidden)
    //        return;

    //    if (Input.touchCount == 0)
    //    {
    //        activeFingerId = -1;
    //        return;
    //    }

    //    foreach (Touch touch in Input.touches)
    //    {
    //        if (touch.position.x < Screen.width / 2f)
    //            continue;

    //        if (touch.phase == TouchPhase.Began)
    //        {
    //            activeFingerId = touch.fingerId;
    //        }
    //        else if (touch.fingerId == activeFingerId && touch.phase == TouchPhase.Moved)
    //        {
    //            HideHint();
    //        }
    //    }
    //}

    private IEnumerator AutoHideHint(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideHint();
    }

    private void HideHint()
    {
        if (hintHidden) return;

        hintHidden = true;

        if (autoHideCoroutine != null)
            StopCoroutine(autoHideCoroutine);

        gameObject.SetActive(false);
    }
}
