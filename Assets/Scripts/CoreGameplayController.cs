using UnityEngine;

public class CoreGameplayController : MonoBehaviour
{
    [SerializeField] private int timerStartValue;
    [SerializeField] private float boxMotionSpeed;

    [SerializeField] private GameObject UIGO;
    [SerializeField] private GameObject TornadoGO;

    [SerializeField] private GameObject blockersGO;
    [SerializeField] private GameObject prefabBoxR;
    [SerializeField] private GameObject prefabBoxG;

    private UIController uiController;
    private TornadoController tornadoC;
    private float timerValue;

    void Start()
    {
        uiController = UIGO.GetComponent<UIController>();
        tornadoC = TornadoGO.GetComponent<TornadoController>();

        if (uiController != null)
        {
            timerValue = timerStartValue;

            uiController.UpdateScore();
            uiController.SetTimerValue((int)timerValue);

            Time.timeScale = 1;
        }
    }

    private void SummonBlocker()
    {
        Vector3 newBlockerPos = new Vector3(Random.Range(tornadoC.boundary.xMin, tornadoC.boundary.xMax), 1F, 12F);
        GameObject newBlocker = Instantiate(GetBlockerPrefab(), newBlockerPos, blockersGO.transform.rotation, blockersGO.transform);
        newBlocker.transform.SetParent(blockersGO.transform);
        newBlocker.GetComponent<Rigidbody>().velocity += Vector3.back * boxMotionSpeed;
    }

    private GameObject GetBlockerPrefab()
    {
        float randomSeed = Random.Range(0F, 1F);

        if (randomSeed >= .5F)
        {
            return prefabBoxG;
        }
        else
        {
            return prefabBoxR;
        }
    }

    void Update()
    {
        UpdateTimer();
        CheckBlockersPosition();
    }

    private void CheckBlockersPosition()
    {
        if (blockersGO.transform.childCount > 0)
        {
            for (int cnt = 0; cnt < blockersGO.transform.childCount; cnt++)
            {
                Transform targetChild = blockersGO.transform.GetChild(cnt);
                if (targetChild.position.z < -12)
                {
                    Destroy(targetChild.gameObject);
                    SummonBlocker();
                }
            }
        }
        else
        {
            SummonBlocker();
        }
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
