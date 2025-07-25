using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoInputSwitcher : MonoBehaviour
{
    [Header("UI / Controls")]
    public GameObject mobileControls;
    public GameObject jumpButtonMobile;

    private void Start()
    {
        if (Application.isMobilePlatform)
        {
            EnableMobileControls();
        }
        //EnableMobileControls();
    }
    void EnableMobileControls()
    {
        if(mobileControls != null) 
        {
            jumpButtonMobile.SetActive(true);
            mobileControls.SetActive(true);
            Debug.Log("Мобильное управление включено");
        }
    }
}
