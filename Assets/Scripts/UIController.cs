using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Text scoreLable;
    [SerializeField] private Text xpLable;
    [SerializeField] private Text tierLable;

    private int scoreNum = 0;

    public void AddScore(int value)
    {
        scoreNum += value;
        UpdateScore();
    }

    public void UpdateScore()
    {
        scoreLable.text = "score: " + scoreNum;
    }

    public void SetXPLable(string value)
    {
        xpLable.text = "XP: " + value;
    }

    public void SetTierLable(string newTier)
    {
        tierLable.text = "Tier: " + newTier;
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
        #if UNITY_ANDROID || UNITY_STANDALONE
        Application.Quit();
        #endif
    }
}