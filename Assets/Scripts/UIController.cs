using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Text score;
    [SerializeField] private Text timer;
    [SerializeField] private Text debugT;

    private int scoreNum = 0;

    public void AddScore(int value)
    {
        scoreNum += value;
        UpdateScore();
    }

    public void UpdateScore()
    {
        score.text = "score: " + scoreNum;
    }

    public void SetTimerValue(int value)
    {
        timer.text = "time: " + value;
    }

    public void SetDebugText(string newText)
    {
        debugT.text = newText;
    }

    public void ShowMenuPanel()
    {
        menuPanel.SetActive(true);
    }

    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitButton()
    {
        #if UNITY_ANDROID
        Application.Quit();
        #endif
    }
}
