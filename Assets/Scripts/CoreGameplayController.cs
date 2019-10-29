using UnityEngine;

public class CoreGameplayController : MonoBehaviour
{
    [SerializeField] private int timerStartValue;
    [SerializeField] private GameObject UIGO;

    private UIController uiController;
    private float timerValue;

    void Start()
    {
        uiController = UIGO.GetComponent<UIController>();

        if (uiController != null)
        {
            timerValue = timerStartValue;

            uiController.SetScoreValue(0);
            uiController.SetTimerValue((int)timerValue);

            Time.timeScale = 1;
        }
    }

    void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (timerValue > 0)
        {
            timerValue -= Time.deltaTime;
            uiController.SetTimerValue((int)timerValue);
        }
        else
        {
            Time.timeScale = 0;
            uiController.ShowMenuPanel();
        }
    }
}
