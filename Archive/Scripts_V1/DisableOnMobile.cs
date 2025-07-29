using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnMobile : MonoBehaviour
{
    private void Awake()
    {
        if (Application.isMobilePlatform)
        {
            gameObject.SetActive(false);
        }
    }
}
