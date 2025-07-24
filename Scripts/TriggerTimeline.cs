using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTimeline : MonoBehaviour
{
    public string nameCutscene;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !GameManager.Instance.HasCutcseneBeenTriggered(nameCutscene))
        {
            Debug.Log("«¿œ”— ¿ﬁ  Œ–»ƒŒ–");
            GameManager.Instance.MarkCutcsenesAsTriggered(nameCutscene);
            CutsceneManager.Instance.StartCutscene(nameCutscene);
        }
    }
    public void GotPosition()
    {
        GameManager.Instance.EnterID(CheckpointManager.Instance.GetLastCheckpointID());
    }
}
