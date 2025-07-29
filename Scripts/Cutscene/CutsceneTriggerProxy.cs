using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTriggerProxy : MonoBehaviour
{
    public void EndCutscene()
    {
        if (CutsceneManager.Instance != null)
        {
            CutsceneManager.Instance.EndCutscene();
        }
    }
}
