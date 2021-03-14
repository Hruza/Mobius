using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Menu : MonoBehaviour
{
    public float sequenceDuration = 1;

    public TextMeshProUGUI score;
    public TextMeshProUGUI topScore;

    public GameObject menuCam;
    public GameObject planeCam;
    public GameObject plane;

    public Transform defaultCamPos;
    public Transform upgradeMenuPos;

    public TextMeshProUGUI[] coinValue;
    static private int currency = 0;
    static public int Currency {
        get {
            return currency;
        }
        set {
            foreach (TextMeshProUGUI text in instance.coinValue)
            {
                text.SetText(value.ToString());
            }
            PlayerPrefs.SetInt("currency",value);
            currency = value;
        }
    }

    static public Menu instance;
    public void Awake()
    {
        instance = this;   
    }

    public void Play()
    {

        StartCoroutine(StartSequence());
    }

    public void Upgrades()
    {
        UpgradeSystem.instance.SetStats();
        StartCoroutine(TransferCamera(menuCam.transform,upgradeMenuPos));
    }

    public void MainMenu() {
        StartCoroutine(TransferCamera(menuCam.transform, defaultCamPos)) ;
    }

    private int topDistance = 0;

    public void Start() {

        if (PlayerPrefs.HasKey("currency"))
        {
            Currency = PlayerPrefs.GetInt("currency");
        }
        else
        {
            Currency = 0;
        }
        if (PlayerPrefs.HasKey("top"))
        {
            topDistance = PlayerPrefs.GetInt("top");
            topScore.SetText($"Top distance {topDistance}");
        }
        else {
            topDistance = 0;
            topScore.SetText("");
        }
        score.SetText("");
    }

    IEnumerator StartSequence() {
        float t = 0;

        plane.SetActive(true);
        PlayerController controller = plane.GetComponent<PlayerController>();

        PlayerController.distance = 0;

        controller.acceleration = UpgradeSystem.Acceleration;
        controller.glidindDeacceleration = UpgradeSystem.Gliding;
        Fuel.maxFuel = UpgradeSystem.Fuel;
        controller.speed = UpgradeSystem.StartingSpeed;
        controller.turningSpeed = UpgradeSystem.Turning;

        Destroy(plane.transform.GetChild(0).gameObject);

        Transform model = Instantiate(UpgradeSystem.Model,plane.transform).transform;
        controller.model = model;
        controller.propeller = model.Find("propeller");

        Fuel.AddFuel(100000);

        Generator.ResetWorld();

        float mobiusTurning=controller.mobiusTurning;

        plane.transform.position = new Vector3(Mobius.instance.radius,1,0) -Vector3.forward * PlayerController.instance.speed * sequenceDuration;
        plane.transform.rotation = Quaternion.identity;

        if (Obstacle.instances != null)
        {
            foreach (GameObject gameObject in Obstacle.instances)
            {
                Destroy(gameObject);
            }
        }

        while (t < sequenceDuration) {
            plane.transform.position += Vector3.forward * Time.fixedDeltaTime * PlayerController.instance.speed;
            menuCam.transform.position = Vector3.Lerp(defaultCamPos.position,planeCam.transform.position,t/sequenceDuration);
            menuCam.transform.rotation = Quaternion.Lerp(menuCam.transform.rotation,planeCam.transform.rotation,t/sequenceDuration);
            model.transform.rotation = Quaternion.Lerp(Quaternion.identity,Quaternion.Euler(0,0,mobiusTurning), t);

            t += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        plane.GetComponent<MobiusTransform>().SetPosition(0, 0, 1);

        menuCam.SetActive(false);
        planeCam.SetActive(true);
        controller.enabled = true;
        controller.drivable = true;

        yield return null;
    }

    private int TopDistance {
        set {
            if (topDistance < value)
            {
                topDistance = value;
                PlayerPrefs.SetInt("top",value);
            }
        }
        get {
            return topDistance;
        }
    }

    public void Exit() {
        Application.Quit();
    }

    public static void BackToMenu() {

        instance.score.SetText($"Your distance {Mathf.RoundToInt(PlayerController.distance)}");
        instance.TopDistance = Mathf.RoundToInt(PlayerController.distance);
        instance.topScore.SetText($"Top distance {instance.TopDistance}");

        instance.plane.GetComponent<PlayerController>().enabled = false;

        PlayerController.distance = 0;

        instance.menuCam.SetActive(true);
        instance.planeCam.SetActive(false);

        PlayerPrefs.SetInt("currency", Currency);
        instance.StartCoroutine(instance.TransferCamera(instance.menuCam.transform,instance.defaultCamPos));
    }

    IEnumerator TransferCamera(Transform move, Transform target)
    {
        float t=0;
        while (t < sequenceDuration)
        {
            menuCam.transform.position = Vector3.Lerp(move.position, target.position, t / sequenceDuration);
            menuCam.transform.rotation = Quaternion.Lerp(move.rotation, target.rotation, t / sequenceDuration);

            t += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        menuCam.transform.position = target.position;
        menuCam.transform.rotation = target.rotation;
    }
}
