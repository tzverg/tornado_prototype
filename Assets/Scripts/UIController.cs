using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Text score;
    [SerializeField] private Text timer;

    public void SetScoreValue(int value)
    {
        score.text = "score: " + value;
    }

    public void SetTimerValue(int value)
    {
        timer.text = "time: " + value;
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
