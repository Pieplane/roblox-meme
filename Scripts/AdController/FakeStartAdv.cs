using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeStartAdv : MonoBehaviour
{
    private void Start()
    {
        AdController.Instance.FakeRespawn(() =>
        {
            Debug.Log("launch");
        });
    }
}
