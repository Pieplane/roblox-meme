using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardYOnly : MonoBehaviour
{
    void LateUpdate()
    {
        Transform cam = Camera.main.transform;
        Vector3 lookDirection = transform.position - cam.position;
        lookDirection.y = 0f; // только по Y
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}
