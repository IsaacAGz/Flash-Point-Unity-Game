using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static PauseMenu I { get; private set; }

    [Header("UI")]
    public GameObject pauseMenuUI;

    bool isPaused = false;
    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            I = this;
        }
        if (pauseMenuUI)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    public void Toggle()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    //function called by PauseButton in PauseMenu   
    public void Pause()
    {
        isPaused = true;
        if (pauseMenuUI)
        {
            pauseMenuUI.SetActive(true);
        }
    }

    //function called by ResumeButton in PauseMenu
    public void Resume()
    {
        isPaused = false;
        if (pauseMenuUI)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    //function called by Restart Button on Pause Menu
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    //fuction called by Exit Button in Pause Menu
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}
