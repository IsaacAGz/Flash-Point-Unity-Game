using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum TurnPhase
{
    Player,
    Game
}
public class TurnManager : MonoBehaviour
{
    public static TurnManager I { get; private set; }
    public int roundNumber { get; private set; } = 1;
    public TurnPhase phase { get; private set; } = TurnPhase.Player;

    public event Action OnPlayerTurnStarted;
    public event Action OnRoundEnded;

    
    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        phase = TurnPhase.Player;
        Debug.Log("Player Turn Started");
        OnPlayerTurnStarted?.Invoke();
    }

    public void EndPlayerTurn()
    {
        Debug.Log("Player Turn Ended");
        EndRound();
    }

    void EndRound()
    {
        Debug.Log("Round Ended");
        phase = TurnPhase.Game;
        OnRoundEnded?.Invoke();
        Invoke(nameof(BeginNewRound), 0.5f);
    }

    void BeginNewRound()
    {
        Debug.Log("New Round Started");
        roundNumber++;
        StartPlayerTurn();
    }

    public void ResetState()
    {
        roundNumber = 1;
        phase = TurnPhase.Player;
    }

    public IEnumerator StartPlayerTurnNextFrame()
    {
        yield return null;
        StartPlayerTurn();
    }

}
