using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTimeline : MonoBehaviour
{
    public string nameCutscene;
    public bool scarySound = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !GameManager.Instance.HasCutcseneBeenTriggered(nameCutscene))
        {
            if(scarySound)
            {
                AudioManager.Instance?.StopPlay("GameMusic");
                AudioManager.Instance?.Play("MeetBarry");
                //scarySound = false;
            }
            //Debug.Log("«¿œ”— ¿ﬁ  Œ–»ƒŒ–");
            GameManager.Instance.MarkCutcsenesAsTriggered(nameCutscene);
            CutsceneManager.Instance.StartCutscene(nameCutscene);
        }
    }
    //public void GotPosition()
    //{
    //    GameManager.Instance.EnterID(CheckpointManager.Instance.GetLastCheckpointID());
    //}

}
