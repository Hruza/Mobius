using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class UpgradeSystem : MonoBehaviour
{

    public const int STAT_COUNT= 5;

    public Vector2[] acceleration;
    public Vector2[] gliding;
    public Vector2[] fuel;
    public Vector2[] startingSpeed;
    public Vector2[] turning;

    public GameObject UpgradeBarPrefab;

    private int accelerationLevel;
    private int glidingLevel;
    private int fuelLevel;
    private int startingSpeedLevel;
    private int turningLevel;

    public GameObject[] models;
    private int currentModelIndex;
    public static GameObject Model {
        get {
            return instance.models[instance.currentModelIndex];
        }
    }

    #region properties
    static public float Acceleration {
        get {
            return instance.acceleration[Mathf.Clamp(instance.accelerationLevel, 0, instance.acceleration.Length - 1)].x;
        }
    }
    static public int AccelerationCost
    {
        get
        {
            return Mathf.RoundToInt(instance.acceleration[Mathf.Clamp(instance.accelerationLevel, 0, instance.acceleration.Length - 1)].y);
        }
    }

    static public float Gliding
    {
        get
        {
            return instance.gliding[Mathf.Clamp(instance.glidingLevel, 0, instance.gliding.Length - 1)].x;
        }
    }
    static public int GlidingCost
    {
        get
        {
            return Mathf.RoundToInt(instance.gliding[Mathf.Clamp(instance.glidingLevel, 0, instance.gliding.Length - 1)].y);
        }
    }

    static public float Fuel
    {
        get
        {
            return instance.fuel[Mathf.Clamp(instance.fuelLevel, 0, instance.fuel.Length - 1)].x;
        }
    }
    static public int FuelCost
    {
        get
        {
            return Mathf.RoundToInt(instance.fuel[Mathf.Clamp(instance.fuelLevel, 0, instance.fuel.Length - 1)].y);
        }
    }

    static public float StartingSpeed
    {
        get
        {
            return instance.startingSpeed[Mathf.Clamp(instance.startingSpeedLevel, 0, instance.startingSpeed.Length - 1)].x;
        }
    }
    static public int StartingSpeedCost
    {
        get
        {
            return Mathf.RoundToInt(instance.startingSpeed[Mathf.Clamp(instance.startingSpeedLevel, 0, instance.startingSpeed.Length - 1)].y);
        }
    }

    static public float Turning
    {
        get
        {
            return instance.turning[Mathf.Clamp(instance.turningLevel, 0, instance.turning.Length - 1)].x;
        }
    }
    static public int TurningCost
    {
        get
        {
            return Mathf.RoundToInt(instance.turning[Mathf.Clamp(instance.turningLevel, 0, instance.turning.Length - 1)].y);
        }
    }
    #endregion

    static public UpgradeSystem instance;
    private void Awake()
    {
        instance = this;
        if (PlayerPrefs.HasKey("acc"))
        {
            accelerationLevel = PlayerPrefs.GetInt("acc");
        }
        else {
            accelerationLevel = 0;
        }

        if (PlayerPrefs.HasKey("gli"))
        {
            glidingLevel = PlayerPrefs.GetInt("gli");
        }
        else
        {
            glidingLevel = 0;
        }

        if (PlayerPrefs.HasKey("fue"))
        {
            fuelLevel = PlayerPrefs.GetInt("fue");
        }
        else
        {
            fuelLevel = 0;
        }

        if (PlayerPrefs.HasKey("spe"))
        {
            startingSpeedLevel = PlayerPrefs.GetInt("spe");
        }
        else
        {
            startingSpeedLevel = 0;
        }

        if (PlayerPrefs.HasKey("tur"))
        {
            turningLevel = PlayerPrefs.GetInt("tur");
        }
        else
        {
            turningLevel = 0;
        }

        if (PlayerPrefs.HasKey("mod"))
        {
            currentModelIndex = PlayerPrefs.GetInt("mod");
        }
        else
        {
            currentModelIndex = 0;
        }

        ChangeModel(0);
    }

    private GameObject shownModel;
    public Transform modelPosition;

    public void ChangeModel(int diff) {
        currentModelIndex = (models.Length+currentModelIndex + diff) % (models.Length);
        PlayerPrefs.SetInt("mod",currentModelIndex);
        if(shownModel!=null) Destroy(shownModel);
        shownModel = Instantiate(models[currentModelIndex],modelPosition.position,modelPosition.rotation,modelPosition);
        shownModel.transform.localScale = Vector3.one * 5;
        shownModel.AddComponent<Rotator>();
        shownModel.AddComponent<SphereCollider>().radius=0.5f;
        shownModel.layer =LayerMask.NameToLayer("MenuCamOnly");
        foreach (Transform child in shownModel.transform)
        {
            child.gameObject.layer= LayerMask.NameToLayer("MenuCamOnly");
        }
    }

    private void Start()
    {
        SetStats();
    }
    public UpgradeBar[] upgradeBars;
    private List<GameObject> bars;

    public void Upgrade(UpgradeBar sender) {
        int index = -1;
        for (int i = 0; i < STAT_COUNT; i++)
        {
            if (sender == upgradeBars[i]) {
                index = i;
            }
        }

        if (index== -1) {
            Debug.LogError("Bar sender not found");
        }

        bool ok = false;
        switch (index)
        {
            case 0:
                if (AccelerationCost<= Menu.Currency) {
                    Menu.Currency -= AccelerationCost;
                    accelerationLevel += 1;
                    ok = true;
                }  
                break;
            case 1:
                if (GlidingCost <= Menu.Currency)
                {
                    Menu.Currency -= GlidingCost;
                    glidingLevel += 1;
                    ok = true;
                }
                break;
            case 2:
                if (FuelCost <= Menu.Currency)
                {
                    Menu.Currency -= FuelCost;
                    fuelLevel += 1;
                    ok = true;
                }
                break;
            case 3:
                if (StartingSpeedCost <= Menu.Currency)
                {
                    Menu.Currency -= StartingSpeedCost;
                    startingSpeedLevel += 1;
                    ok = true;
                }
                break;
            case 4:
                if (TurningCost <= Menu.Currency)
                {
                    Menu.Currency -= TurningCost;
                    turningLevel += 1;
                    ok = true;
                }
                break;
            default:
                break;
        }

        if (ok)
        {
            SaveStats();
            SetStats();
        }
        else {
            Debug.Log("Not enough money");
        }
    }

    public void SaveStats() {
        PlayerPrefs.SetInt("acc",accelerationLevel);
        PlayerPrefs.SetInt("gli", glidingLevel);
        PlayerPrefs.SetInt("fue", fuelLevel);
        PlayerPrefs.SetInt("spe", startingSpeedLevel);
        PlayerPrefs.SetInt("tur", turningLevel);
    }

    public void SetStats()
    {
        upgradeBars[0].SetValue("Acceleration",accelerationLevel,acceleration.Length,AccelerationCost);

        upgradeBars[1].SetValue("Gliding", glidingLevel, gliding.Length, GlidingCost);

        upgradeBars[2].SetValue("Fuel", fuelLevel, fuel.Length, FuelCost);

        upgradeBars[3].SetValue("Starting Speed", startingSpeedLevel, startingSpeed.Length,StartingSpeedCost);

        upgradeBars[4].SetValue("Turning", turningLevel, turning.Length, TurningCost);
    }

}
