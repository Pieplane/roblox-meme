using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class StartAdv : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ShowAdv();

    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ShowAdv();
#endif
    }
}
