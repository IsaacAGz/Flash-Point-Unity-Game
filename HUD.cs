using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class HUD : MonoBehaviour
{
    [Header("References")]
    public PlayerAP ap;
    public TextMeshProUGUI apText;
    public TextMeshProUGUI scoreText;


    // Start is called before the first frame update
    void Start()
    {
        if (!ap) ap = FindObjectOfType<PlayerAP>();
        if (ScoreManager.I) ScoreManager.I.OnScoreChanged += s => UpdateScore(s);

        UpdateAP();
        UpdateScore(ScoreManager.I ? ScoreManager.I.Score : 0);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAP();
    }

    void OnDestroy()
    {
        if (ScoreManager.I) ScoreManager.I.OnScoreChanged -= s => UpdateScore(s);
    }

    //Updates AP Text
    void UpdateAP()
    {
        if (apText && ap)
        {
            apText.text = $"AP: {ap.currentAP}/{ap.maxAP}";
        }
    }

    //Updates Score Text
    void UpdateScore(int score)
    {
        if (scoreText)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    //Function Called by Menu Button
    public void OnMenuButton()
    {
        PauseMenu.I?.Toggle();
    }
}
