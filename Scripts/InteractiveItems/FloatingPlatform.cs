using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatform : MonoBehaviour
{
    [Header("Движение")]
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    [Header("Парение")]
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 1f;

    [Header("Рассинхронизация")]
    public float timeOffset = 0f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;

        if(Mathf.Approximately(timeOffset, 0f))
        {
            timeOffset = Random.Range(0f, 100f);
        }
    }
    private void Update()
    {
        float t = Time.time + timeOffset;

        float horizontalOffset = Mathf.Sin(t * moveSpeed) * moveDistance;
        float verticalOffset = Mathf.Sin(t * floatFrequency) * floatAmplitude;

        transform.position = startPosition + new Vector3(horizontalOffset, verticalOffset, 0f);
    }
}
