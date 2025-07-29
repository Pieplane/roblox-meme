using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentInteraction : MonoBehaviour
{
    public Transform player;
    public float interactionDistance = 2f;
    public Rigidbody ventRigidbody;
    public Vector3 fallForceDirection = new Vector3(0,0,1);
    public float fallForce;
    public GameObject textCanvas;

    private bool hasFallen = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool initialIsKinematic;
    private bool initialUseGravity;

    private void Start()
    {
        if (ventRigidbody != null)
        {
            initialPosition = ventRigidbody.transform.position;
            initialRotation = ventRigidbody.transform.rotation;
            initialIsKinematic = ventRigidbody.isKinematic;
            initialUseGravity = ventRigidbody.useGravity;
        }
    }

    private void Update()
    {
        if (hasFallen || player == null || textCanvas == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        bool isNear = distance <= interactionDistance;

        textCanvas.SetActive(isNear);
        if(isNear)
        {
            //Debug.Log("Can move Lattice");
            if (Input.GetKeyDown(KeyCode.E))
            {
                Fall();
            }

        }
        
    }
    public void ResetVent()
    {
        hasFallen = false;
        ventRigidbody.velocity = Vector3.zero;
        ventRigidbody.angularVelocity = Vector3.zero;
        ventRigidbody.transform.position = initialPosition;
        ventRigidbody.transform.rotation = initialRotation;
        ventRigidbody.isKinematic = initialIsKinematic;
        ventRigidbody.useGravity = initialUseGravity;

        if (textCanvas != null)
            textCanvas.SetActive(false);
    }
    public void Fall()
    {
        hasFallen = true;
        ventRigidbody.isKinematic = false;
        ventRigidbody.useGravity = true;
        ventRigidbody.AddForce(fallForceDirection.normalized*fallForce, ForceMode.Impulse);
        textCanvas.SetActive(false);
    }
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawSphere(transform.position, interactionDistance);
    //}
}
