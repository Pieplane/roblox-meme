using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnPC : MonoBehaviour
{
    private void Awake()
    {
        if (!Application.isMobilePlatform)
        {
            gameObject.SetActive(false);
        }
    }
}
