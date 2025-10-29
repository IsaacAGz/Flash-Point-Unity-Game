using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager I { get; private set; }
    
    public int Score { get; private set; }
    public int BestScore { get; private set; }

    public event Action<int> OnScoreChanged;

    public AudioSource audioSource;
    public AudioClip getPointsClip;
    
    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(this.gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(this.gameObject);
    }

    //Adds points to Score int and plays audio file to scoring points
    public void AddPoints(int points)
    {
        Score += points;
        audioSource.PlayOneShot(getPointsClip);
        OnScoreChanged?.Invoke(Score);
        Debug.Log($"Added points: {points}");
    }

    //Resets Score to zero when restart
    public void ResetScore()
    {
        Score = 0;
        OnScoreChanged?.Invoke(Score);
    }

    //Attemps to change High Score with Score
    public void TrySetHighScore(int score)
    {
        if (score > BestScore)
        {
            BestScore = score;
            PlayerPrefs.SetInt("HighScore", BestScore);
            PlayerPrefs.Save();
        }
    }
}
