using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MobiusTransform))]
public class PlayerController : MonoBehaviour
{

    private MobiusTransform tr;
    public Transform model;
    public Transform propeller;

    public GameObject OnDeathParticles;

    public GameObject Model {
        set {
            if (model != null) {
                Destroy(model);
            }
            model = Instantiate(value, transform).transform;
        }
    }

    [Header("Movement")]
    public float speed = 1;
    public float turningSpeed = 1;
    public float rot = 1f;
    public float maxRot = 60f;
    [Range(0, 1)]
    public float balanceSpeed;
    public float acceleration = 0.1f;
    public float glidindDeacceleration = 0.5f;

    [Header("Effects")]
    public float turbulence = 0.1f;
    public float mobiusTurning = 1;
    public float propellerRotationSpeed = 1;

    private float currentRot;

    public static PlayerController instance;

    public static void SlowDown(float value) {
        instance.speed = Mathf.Max(0, instance.speed-value);
    }

    private void Awake()
    {
        tr = GetComponent<MobiusTransform>();
        this.enabled = false;
        if (instance == null)
        {
            instance = this;
        }
        else {
            Destroy(this);
        }
    }

    float t = 0;
    private void Start()
    {
        t = 0;
        audioSource = GetComponent<AudioSource>();
    }


    #region audio
    private AudioSource audioSource;
    private float targetPitch = 1;
    private float targetVolume = 1;

    private float propellerSpeed = 1;

    private void Update()
    {
        propellerSpeed = Mathf.Lerp(audioSource.pitch, targetPitch, pitchChangeSpeed);
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, volumeChangeSpeed);
        audioSource.pitch = propellerSpeed;
    }

    [Header("audio")]
    public float turningSensitivity=0.01f;
    public float speedSensitivity = 0.02f;
    public float startingPitch = 0.8f;

    public float pitchChangeSpeed = 0.3f;
    public float volumeChangeSpeed = 0.1f;

    private void CalculateSound(bool hasFuel, float speed, float turning) {
        if (!hasFuel)
        {
            targetVolume = 0;
            targetPitch = 0.1f;
        }
        else {
            targetVolume = 0.5f;
            targetPitch = startingPitch + (speed * speedSensitivity ) + (Mathf.Abs(turning)*turningSensitivity);
        }
    }

    #endregion

    float naturalTurning = 0;
    static public float distance = 0;

    static public float LoopCount {
        get {
            return distance / (Mobius.instance.radius * 2*Mathf.PI);
        }
    }

    private float TouchInput() {
        if (Input.touchCount==0) {
            return 0;
        }
        else {
            Touch t = Input.GetTouch(0);
            return Mathf.Clamp(0.1f*(t.position.x-(Screen.width/2)),-1,1);
        }
    }

    public bool drivable = true;
    private void FixedUpdate()
    {
        if (!drivable) {
            return;
        }
        distance += speed * Time.fixedDeltaTime;
        tr.Fi = distance/Mobius.instance.radius;
        Distance.Value = distance;

        if (Fuel.HasFuel)
        {
            speed += acceleration * Time.fixedDeltaTime / Mathf.Max(speed, 1);
            Fuel.UseFuel(Time.fixedDeltaTime);
        }
        else if (speed > 0.5f)
        {
            speed = Mathf.Max(speed - (glidindDeacceleration * Time.deltaTime), 0);
        }
        else {
            drivable = false;
            StartCoroutine(DeathSequence());
        }
        Speedometer.SetSpeed(speed);

        if (Input.touchCount == 0)
        {
            currentRot -= Input.GetAxisRaw("Horizontal") * rot;
        }
        else
        {
            currentRot -= TouchInput() * rot;
        }
        CalculateSound(Fuel.HasFuel,speed,currentRot);

        RotatePropeller(propellerRotationSpeed);
        
        currentRot = Mathf.Clamp(currentRot, -maxRot, + maxRot);

        tr.D -= (currentRot * turningSpeed * Time.deltaTime);
        
        naturalTurning = Mathf.Cos(tr.Fi)*mobiusTurning;
        
        t += Time.deltaTime;
        model.localPosition = turbulence*(new Vector3(Mathf.PerlinNoise(t,0)-0.5f,Mathf.PerlinNoise(t,10)-0.5f,0));
        model.localRotation = Quaternion.Euler(-10,0,naturalTurning+currentRot);
        currentRot = Mathf.Lerp(currentRot,0,balanceSpeed);
    }


    void RotatePropeller(float speed) {
        propeller.Rotate(speed*(propellerSpeed-0.1f),0,0);
    }

    private IEnumerator DeathSequence() {
        float t= 0;
        while (t < 1) {
            model.transform.localPosition = new Vector3(t*(Mathf.PerlinNoise(t, 0) - 0.5f),Mathf.Max(-2*t*(t-0.5f),-1), t);
            model.transform.localRotation = Quaternion.Euler((50*(t+1)*(t-0.8f)),0,180*t);

            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        Destroy(Instantiate(OnDeathParticles,transform.position,transform.rotation),4);
        CameraShake.DoShake();
        yield return new WaitForSeconds(1f);
        Menu.BackToMenu();
    }
}
