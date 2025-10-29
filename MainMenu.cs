
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameplaySceneName = "GameplayScene";
    public string MainMenuScene = "MainMenuScene";

    public void OnPlayClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnQuitClicked()
    {
        SceneManager.LoadScene(MainMenuScene);
    }
}
