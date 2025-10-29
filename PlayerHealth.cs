using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public int maxHealth = 1;
    public int currentHP { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHealth;
    }

    
    public void TakeDamage(int damage)
    {
        GameOverManager.I?.Lose();
    }
}
