using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Text scoreLable;
    [SerializeField] private Text xpLable;
    [SerializeField] private Text tierLable;

    public void SetScoreLable(int value)
    {
        scoreLable.text = string.Format("{0}", value);
    }

    public void SetXPLable(int level, int value, int max)
    {
        tierLable.text = string.Format("Level: {0}", level);
        xpLable.text = string.Format("XP:{0}/{1}", value, max);
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