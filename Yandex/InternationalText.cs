using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InternationalText : MonoBehaviour
{
    [SerializeField] string _en;
    [SerializeField] string _tr;
    [SerializeField] string _ru;

    private void Start()
    {
        string lang = Language.Instance?.CurrentLanguage ?? "en";
        //string lang = "ru";
        switch (lang)
        {
            case "en":
                GetComponent<TextMeshProUGUI>().text = _en;
                break;
            case "ru":
                GetComponent<TextMeshProUGUI>().text = _ru;
                break;
            case "tr":
                GetComponent<TextMeshProUGUI>().text = _tr;
                break;
            default:
                GetComponent<TextMeshProUGUI>().text = _en;
                break;

        }
    }

}
