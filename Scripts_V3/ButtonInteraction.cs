using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class ButtonInteraction : MonoBehaviour
{
    public Transform player;
    public float interactionDistance = 2f;
    public GameObject textCanvas;
    public string nameCutscene;
    private bool hasFallen = false;

    private void Update()
    {
        if (hasFallen || player == null || textCanvas == null || CheckpointManager.Instance.IsCheckpointReached(nameCutscene)) return;

        float distance = Vector3.Distance(player.position, transform.position);
        bool isNear = distance <= interactionDistance;

        textCanvas.SetActive(isNear);

        if (isNear)
        {
            //Debug.Log("Can move Lattice");
            if (Input.GetKeyDown(KeyCode.E))
            {
                Fall();
                Debug.Log("√отов открывть и запускать анимацию");
                CheckpointManager.Instance.SetCheckpoint(nameCutscene);
            }

        }
    }
    
    public void Fall()
    {
        hasFallen = true;
        
        CutsceneManager.Instance.StartCutscene(nameCutscene);
        textCanvas.SetActive(false);
        
    }
    
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawSphere(transform.position, interactionDistance);
    //}
}
