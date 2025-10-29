using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayBoostrap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (TurnManager.I)
        {
            TurnManager.I.ResetState();
            StartCoroutine(TurnManager.I.StartPlayerTurnNextFrame());
        }
    }

}
