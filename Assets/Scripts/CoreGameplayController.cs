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
    public int[] xpTiers = { 5, 10, 15, 20 };

    public TornadoType tornadoType;
	
	//TODO: change TierType to int
    public TierType tornadoTier;
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

    void Start()
    {
        uiController = UIGO.GetComponent<UIController>();

        if (uiController != null)
        {
            timerValue = config.timerStartValue;

            uiController.UpdateScore();
            uiController.SetTierLable(config.tornadoTier.ToString());
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

    public void AddXP(int xpValue)
    {
        config.currentXP += xpValue;
    }

    private void UpdateXPLable()
    {
        int tierXP = config.xpTiers[(int)config.tornadoTier];
        string resultValue = config.currentXP + " / " + tierXP;
        uiController.SetXPLable(resultValue);
    }

    public void CreateTornado(Vector3 tornadoPos, Vector3 tornadoScale/*, TornadoType newType*/)
    {
        TornadoType newType = GetRandomTornado();   //For Test

        config.tornadoType = newType;

        GameObject newTornado = Instantiate(GetTornadoPrefab(newType));
        newTornado.transform.localScale = tornadoScale;

        TornadoController newTornadoC = newTornado.GetComponent<TornadoController>();
        newTornadoC.UIGO = UIGO;
        newTornadoC.CoreGameplayCGO = gameObject;
        newTornadoC.tornadoScale = tornadoScale.x;

        UpdateTornadoTier();
    }

    public bool UpdateTornadoTier()
    {
        //for (int cnt = 0; cnt < config.scaleLevel.Length; cnt++)
        //{
        //    if (tornadoScale >= config.scaleLevel[cnt])
        //    {
        //        config.tornadoTier = (TierType)cnt;
        //    }
        //}

        int tornadoTierID = (int)config.tornadoTier;
        int enumLenght = Enum.GetValues(typeof(TierType)).Length;

        if (config.currentXP >= config.xpTiers[tornadoTierID])
        {
            if (++tornadoTierID < enumLenght)
            {
                config.tornadoTier = (TierType)tornadoTierID;

                if (tornadoTierID == (enumLenght - 1))
                {
                    uiController.SetXPLable("max");
                    EndTheGame();
                }
                else
                {
                    UpdateBlockerPrefab();
                    UpdateXPLable();
                }
                uiController.SetTierLable(config.tornadoTier.ToString());

                return true;
            }
        }

        UpdateXPLable();

        return false;
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

    private GameObject GetBlocker(TierType newBlockerTier)
    {
        if (newBlockerTier > config.tornadoTier)
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
        float randomSeed = UnityEngine.Random.Range(0F, 1F);

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

    private TierType GetRandomBlockerTier()
    {
        float randomSeed = UnityEngine.Random.Range(0F, 1F);
        float dropChance = 0F;
        int enumLenght = Enum.GetValues(typeof(TierType)).Length;
        int tornadoTierID = (int)config.tornadoTier;
        int diff = 0;
        int blockerTierID = 0;
        bool isBorderTier = GetIsBorderTier(tornadoTierID, enumLenght);

        for (int cnt = 0; cnt < enumLenght; cnt++)
        {
            diff = Mathf.Abs(tornadoTierID - cnt);

            dropChance = GetDropChance(isBorderTier, diff);
            if (randomSeed < dropChance)
            {
                //Debug.Log("Chance: " + dropChance + ", Type: " + (TierType)blockerTierID);

                blockerTierID = cnt;
            }
        }

        return (TierType)blockerTierID;
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

    private bool GetIsBorderTier(int tornadoTierID, int maxTierID)
    {
        if (tornadoTierID == 0 || tornadoTierID == maxTierID)
        {
            return true;
        }
        else
        {
            return false;
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

    public void UpdateBlockerPrefab()
    {
        int tornadoTierID = (int)config.tornadoTier;
        for (int cnt = 0; cnt < prefabs.blockersGO.transform.childCount; cnt++)
        {
            GameObject blockerGO = prefabs.blockersGO.transform.GetChild(cnt).gameObject;
            int blockerTierID = (int)blockerGO.GetComponent<BlockerModel>().blockerTier;

            if (blockerGO.tag == "RedMarker" && tornadoTierID >= blockerTierID)
            {
                Vector3 newBlockerPos = blockerGO.transform.position;

                Destroy(blockerGO);

                GameObject newBlocker = Instantiate(GetBlocker((TierType)blockerTierID), prefabs.blockersGO.transform);
                newBlocker.GetComponent<BlockerModel>().blockerTier = (TierType)blockerTierID;
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
                TierType newBlockerTier = GetRandomBlockerTier();

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
        Vector3 currLocalScale = targetBlocker.transform.localScale;

        switch (targetBlockerM.blockerTier)
        {
            case TierType.Great:
                currLocalScale *= config.scaleLevel[3];
                break;
            case TierType.Big:
                currLocalScale *= config.scaleLevel[2];
                break;
            case TierType.Medium:
                currLocalScale *= config.scaleLevel[1];
                break;
            case TierType.Small:
                currLocalScale *= config.scaleLevel[0];
                break;
        }
        return currLocalScale;
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
