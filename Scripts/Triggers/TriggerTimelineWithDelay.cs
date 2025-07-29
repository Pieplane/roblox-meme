using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTimelineWithDelay : MonoBehaviour
{
    public string nameCutscene;
    public string nextCutsceneID;
    public bool scarySound = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !GameManager.Instance.HasCutcseneBeenTriggered(nameCutscene))
        {
            if (scarySound)
            {
                AudioManager.Instance?.StopPlay("GameMusic");
                AudioManager.Instance?.Play("MeetBarry");
                //scarySound = false;
            }
            //Debug.Log("ÇÀÏÓÑÊÀÞ ÊÎÐÈÄÎÐ");
            GameManager.Instance.MarkCutcsenesAsTriggered(nameCutscene);
            //CutsceneManager.Instance.StartCutscene(nameCutscene);

            CutsceneManager.Instance.StartCutscene(nameCutscene, () =>
            {
                GameManager.Instance.EnterID("Toilet");
            });
        }
    }

}
