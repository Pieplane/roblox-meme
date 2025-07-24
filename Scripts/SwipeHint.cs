using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeHint : MonoBehaviour
{
    private bool hintHidden = false;
    private int activeFingerId = -1;

    private void Start()
    {
        gameObject.SetActive(true); // ���������� ��������� ��� ������
    }

    private void Update()
    {
        if (hintHidden)
            return;

        if (Input.touchCount == 0)
        {
            activeFingerId = -1;
            return;
        }

        foreach (Touch touch in Input.touches)
        {
            // ��������� ������ �� ������ �� ������ ����� ������
            if (touch.position.x < Screen.width / 2f)
                continue;

            if (touch.phase == TouchPhase.Began)
            {
                activeFingerId = touch.fingerId;
            }
            else if (touch.fingerId == activeFingerId && touch.phase == TouchPhase.Moved)
            {
                HideHint();
            }
        }
    }

    private void HideHint()
    {
        hintHidden = true;
        gameObject.SetActive(false); // �������� ���� ���������
    }
}
