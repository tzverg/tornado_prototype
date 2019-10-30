using System;
using UnityEngine;

public enum TornadoType { Lightning, Ice, Fire };

[Serializable]
public class Configurations
{
    public float xMin, xMax, zMin, zMax;
    public float modR, modG;
    public float tornadoMotionSpeed;
    public TornadoType tornadoT;
}

[Serializable]
public class BlockerModifiers
{
    public float modR, modG;
}

public class CoreGameplayController : MonoBehaviour
{
    [SerializeField] private int timerStartValue;
    [SerializeField] private float boxMotionSpeed;

    [SerializeField] private GameObject UIGO;

    [SerializeField] private GameObject blockersGO;
    [SerializeField] private GameObject prefabBoxR;
    [SerializeField] private GameObject prefabBoxG;
    [SerializeField] private GameObject prefabBoxB;

    [SerializeField] private GameObject prefabTornadoL;
    [SerializeField] private GameObject prefabTornadoF;
    [SerializeField] private GameObject prefabTornadoI;

    public BlockerModifiers modifiers;
    public Configurations config;

    private UIController uiController;
    private float timerValue;

    void Start()
    {
        uiController = UIGO.GetComponent<UIController>();

        if (uiController != null)
        {
            timerValue = timerStartValue;

            uiController.UpdateScore();
            uiController.SetTimerValue((int)timerValue);

            Time.timeScale = 1;
        }

        CreateTornado(new Vector3(0F, 0F, -12F));
    }

    private void SummonBlocker()
    {
        Vector3 newBlockerPos = new Vector3(UnityEngine.Random.Range(config.xMin, config.xMax), 1F, 12F);
        GameObject newBlocker = Instantiate(GetBlockerPrefab(), newBlockerPos, blockersGO.transform.rotation, blockersGO.transform);
        newBlocker.transform.SetParent(blockersGO.transform);
        newBlocker.GetComponent<Rigidbody>().velocity += Vector3.back * boxMotionSpeed;
    }

    public void CreateTornado(Vector3 tornadoPos/*, TornadoType newType*/)
    {
        TornadoType newType = GetRandomTornado();   //For Test

        config.tornadoT = newType;

        GameObject newTornado = Instantiate(GetTornadoPrefab(newType));
        newTornado.transform.position = tornadoPos;

        newTornado.AddComponent<Rigidbody>().useGravity = false;

        TornadoController newTornadoC = newTornado.AddComponent<TornadoController>();
        newTornadoC.UIGO = UIGO;
        newTornadoC.CoreGameplayCGO = gameObject;

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.SetParent(newTornado.transform);
        cylinder.transform.localPosition = Vector3.forward;
        cylinder.transform.localRotation = Quaternion.Euler(90F, 0F, 0F);
        cylinder.transform.localScale = new Vector3(2F, 1F, 2F);

        cylinder.GetComponent<MeshRenderer>().enabled = false;
    }

    private GameObject GetTornadoPrefab(TornadoType newType)
    {
        switch (newType)
        {
            case TornadoType.Lightning:
                return prefabTornadoL;
            case TornadoType.Fire:
                return prefabTornadoF;
            case TornadoType.Ice:
                return prefabTornadoI;
            default:
                return prefabTornadoL;
        }
    }

    private TornadoType GetRandomTornado()
    {
        float randomSeed = UnityEngine.Random.Range(0F, 1F);

        if (randomSeed >= .66F)
        {
            return TornadoType.Fire;
        }
        else if (randomSeed >= .33F)
        {
            return TornadoType.Ice;
        }
        else
        {
            return TornadoType.Lightning;
        }
    }

    private GameObject GetBlockerPrefab()
    {
        float randomSeed = UnityEngine.Random.Range(0F, 1F);

        if (randomSeed >= .66F)
        {
            return prefabBoxG;
        }
        else if(randomSeed >= .33F)
        {
            return prefabBoxR;
        }
        else
        {
            return prefabBoxB;
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
            EndTheGame();
        }
    }

    public void EndTheGame()
    {
        Time.timeScale = 0;
        uiController.ShowMenuPanel();
    }
}
