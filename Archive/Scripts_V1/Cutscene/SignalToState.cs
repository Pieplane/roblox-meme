using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalToState : MonoBehaviour
{
    [SerializeField] private GameState targetState;
    
    public void Trigger()
    {
        //GameManager.Instance.EnterID("");
    }
}
