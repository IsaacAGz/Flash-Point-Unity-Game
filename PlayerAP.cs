using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAP : MonoBehaviour
{
    public int maxAP = 5;
    public int currentAP { get; private set; }

    void OnEnable()
    {
        TrySubscribe();
    }
    void OnDisable()
    {
        TryUnsuscribe();
    }

    void Start()
    {
        if (TurnManager.I != null && TurnManager.I.phase == TurnPhase.Player && currentAP == 0)
        {
            TrySubscribe();
        }
    }

    void TryUnsuscribe()
    {
        if (TurnManager.I != null)
        {
            Debug.Log("PlayerAP unsubscribed from OnPlayerTurnStarted");
            TurnManager.I.OnPlayerTurnStarted -= Refill;
        }
    }

    void TrySubscribe()
    {
        if (TurnManager.I != null)
        {
            Debug.Log("PlayerAP subscribed to OnPlayerTurnStarted");
            TryUnsuscribe();   
            TurnManager.I.OnPlayerTurnStarted += Refill;
        }
    }

    public void Refill()
    {
        currentAP = maxAP;
        Debug.Log($"Player AP refilled to {currentAP}");
    }

    public bool CanSpend(int cost)
    {
        return currentAP >= cost;
    }
    public bool Spend(int cost)
    {
        if (!CanSpend(cost)) return false;
        currentAP -= cost;
        return true;
    }
}
