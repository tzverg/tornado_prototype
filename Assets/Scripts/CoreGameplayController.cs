using System;
using System.Collections;
using UnityEngine;

public enum TornadoType { Lightning, Ice, Fire };
public enum TierType { Small, Medium, Big, Great };

[Serializable]
public class Borders
{
    public float xMin, xMax, zMin, zMax;
}
[Serializable]
public class Configurations
{
    public Borders borders;

    public float modR, modG;
    public float tornadoMotionSpeed;
    public float minSwipeDistance;
    public float summonDelayMin, summonDelayMax, blockerMaxNumber;
    public float boxMotionSpeed;

    public float[] chancesB;
    public float[] chancesNB;
    public float[] scaleLevel;

    public int timerStartValue;
    public int currentXP = 0;

	//TODO: change it to progreesive by xpMultipler
    //public int[] xpTiers = { 20, 10, 15, 20 };

    public TornadoType tornadoType;
	
	//TODO: change TierType to int
    public int tornadoTier;
}

[Serializable]
public class Prefabs
{
    public GameObject blockersGO;

    public GameObject blockerRGO;
    public GameObject blockerGGO;
    public GameObject blockerBGO;

    public GameObject tornadoLGO;
    public GameObject tornadoFGO;
    public GameObject tornadoIGO;
}

public class CoreGameplayController : MonoBehaviour
{
    [SerializeField] private GameObject UIGO;

    public Configurations config;
    public Prefabs prefabs;

    private UIController uiController;
    private float timerValue;

    private int _scores = 0;
    private int _tornadoLevel = 1;

    void Start()
    {
        uiController = UIGO.GetComponent<UIController>();

        if (uiController != null)
        {
            //timerValue = config.timerStartValue;

            //uiController.UpdateScore();
            //uiController.SetTierLable(config.tornadoTier.ToString());
            //uiController.SetTimerValue((int)timerValue);

            Time.timeScale = 1;
        }

        CreateTornado(new Vector3(0F, 0F, -12F), Vector3.one);
        StartCoroutine(SummonBlockers());
    }

    static T RandomEnumValue<T>()
    {
        Array v = Enum.GetValues(typeof(T));
        return (T)v.GetValue(new System.Random().Next(v.Length));
    }

    public void AddScores(int value)
    {
        _scores += value;
        uiController.SetScoreLable(_scores);
    }

    public void CreateTornado(Vector3 tornadoPos, Vector3 tornadoScale/*, TornadoType newType*/)
    {
        //TornadoType newType = GetRandomTornado();   //For Test
        TornadoType newType = TornadoType.Lightning;   //For Test

        //config.tornadoType = newType;

        GameObject newTornado = Instantiate(GetTornadoPrefab(newType));
        newTornado.transform.localScale = tornadoScale;

        TornadoController newTornadoC = newTornado.GetComponent<TornadoController>();
        newTornadoC.Init(this, uiController, tornadoScale.x);
    }

    private GameObject GetTornadoPrefab(TornadoType newType)
    {
        switch (newType)
        {
            case TornadoType.Lightning:
                return prefabs.tornadoLGO;
            case TornadoType.Fire:
                return prefabs.tornadoFGO;
            case TornadoType.Ice:
                return prefabs.tornadoIGO;
            default:
                return prefabs.tornadoLGO;
        }
    }

    private TornadoType GetRandomTornado()
    {
        float randomSeed = UnityEngine.Random.value;

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

    private GameObject GetBlocker(int newBlockerTier)
    {
        if (newBlockerTier > _tornadoLevel)
        {
            return prefabs.blockerRGO;
        }
        else
        {
            return prefabs.blockerGGO;
        }
    }

    private GameObject GetRandomBlocker()
    {
        float randomSeed = UnityEngine.Random.value;

        if (randomSeed >= .5F)
        {
            return prefabs.blockerGGO;
        }
        //else if(randomSeed >= .3F)
        //{
        //    return prefabs.blockerBGO;
        //}
        else
        {
            return prefabs.blockerRGO;
        }
    }

    private int GetRandomBlockerTier()
    {
        return UnityEngine.Random.Range(1, _tornadoLevel + 3);
    }

    private float GetDropChance(bool isBorderTier, int diff)
    {
        if (diff < config.chancesB.Length)
        {
            if (isBorderTier)
            {
                return config.chancesB[diff];
            }
            else
            {
                return config.chancesNB[diff];
            }
        }
        else
        {
            Debug.LogError("out of bounds array");
            return 0F;
        }
    }

    void Update()
    {
        //UpdateTimer();
        CheckBlockersPosition();
    }

    private void CheckBlockersPosition()
    {
        for (int cnt = 0; cnt < prefabs.blockersGO.transform.childCount; cnt++)
        {
            Transform targetChild = prefabs.blockersGO.transform.GetChild(cnt);
            if (targetChild.position.z < (config.borders.zMin - 2F))
            {
                Destroy(targetChild.gameObject);
            }
        }
    }

    public void UpdateBlockerPrefab(int level)
    {
        _tornadoLevel = level;

        for (int cnt = 0; cnt < prefabs.blockersGO.transform.childCount; cnt++)
        {
            GameObject blockerGO = prefabs.blockersGO.transform.GetChild(cnt).gameObject;
            int blockerTierID = (int)blockerGO.GetComponent<BlockerModel>().blockerTier;

            if (blockerGO.tag == "RedMarker" && level >= blockerTierID)
            {
                Vector3 newBlockerPos = blockerGO.transform.position;

                Destroy(blockerGO);

                GameObject newBlocker = Instantiate(GetBlocker(blockerTierID), prefabs.blockersGO.transform);
                newBlocker.GetComponent<BlockerModel>().blockerTier = blockerTierID;
                newBlocker.transform.position = newBlockerPos;
                newBlocker.transform.localScale = GetBlockerScale(newBlocker);
                newBlocker.GetComponentInChildren<Rigidbody>().velocity += Vector3.back * config.boxMotionSpeed;
            }
        }
    }

    private IEnumerator SummonBlockers()
    {
        while (true)
        {
            if (prefabs.blockersGO.transform.childCount < config.blockerMaxNumber)
            {
                Vector3 newBlockerPos = new Vector3(UnityEngine.Random.Range(config.borders.xMin, config.borders.xMax), 0F, 12F);
                //TierType newBlockerTier = RandomEnumValue<TierType>();
                int newBlockerTier = GetRandomBlockerTier();

                GameObject newBlocker = Instantiate(GetBlocker(newBlockerTier), newBlockerPos, prefabs.blockersGO.transform.rotation, prefabs.blockersGO.transform);
                BlockerModel newBlockerModel = newBlocker.GetComponent<BlockerModel>();
                newBlockerModel.blockerTier = newBlockerTier;
                newBlockerModel.XpForDestroy = (int)newBlockerTier + 1;
                newBlocker.transform.SetParent(prefabs.blockersGO.transform);
                newBlocker.transform.localScale = GetBlockerScale(newBlocker);
                newBlocker.transform.localRotation = Quaternion.Euler(0F, UnityEngine.Random.Range(0, 360F), 0F);
                newBlocker.GetComponentInChildren<Rigidbody>().velocity += Vector3.back * config.boxMotionSpeed;
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(config.summonDelayMin, config.summonDelayMax));
        }
    }

    private Vector3 GetRandomScale(Vector3 currLocalScale)
    {
        float randomSeed = UnityEngine.Random.Range(0.5F, 2F);
        currLocalScale *= randomSeed;
        return currLocalScale;
    }

    private Vector3 GetBlockerScale(GameObject targetBlocker)
    {
        BlockerModel targetBlockerM = targetBlocker.GetComponent<BlockerModel>();
        float currScale = targetBlocker.transform.localScale .x + 0.25f * targetBlockerM.blockerTier;

        return new Vector3(currScale, currScale * 0.01f, currScale);
    }

    //private void UpdateTimer()
    //{
    //    if (timerValue > 0)
    //    {
    //        timerValue -= Time.deltaTime;
    //        uiController.Set((int)timerValue);
    //    }
    //    else
    //    {
    //        EndTheGame();
    //    }
    //}

    public void EndTheGame()
    {
        Time.timeScale = 0;
        uiController.ShowMenuPanel();
    }
}
