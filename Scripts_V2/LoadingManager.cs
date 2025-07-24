using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public string sceneToLoad; // �������� �����, ������� ����� ���������
    public GameObject spinnerObject;         // ������, ������� �������� (��������, ������ ��������)

    public float rotationSpeed = 200f;

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private void Update()
    {
        if (spinnerObject != null)
        {
            spinnerObject.transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        // ��� ���������� ��������
        while (!operation.isDone)
        {
            yield return null;
        }
    }
}
