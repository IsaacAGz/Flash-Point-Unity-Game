using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class GameOverManager : MonoBehaviour
{

    public static GameOverManager I { get; private set; }
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    public void Lose()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);

        int score = ScoreManager.I ? ScoreManager.I.Score : 0;
        if (finalScoreText) finalScoreText.text = $"Final Score: {score}";

        if (ScoreManager.I && highScoreText)
        {
            ScoreManager.I.TrySetHighScore(score);
            highScoreText.text = $"High Score: {ScoreManager.I.BestScore}";
        }
        
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        if(ScoreManager.I) ScoreManager.I.ResetScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}