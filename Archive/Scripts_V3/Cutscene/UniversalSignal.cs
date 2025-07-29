using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalSignal : MonoBehaviour
{
    public void GoToWaitingToWake()
    {
        GameManager.Instance.SetState(GameState.WaitingToWake);
    }
}
