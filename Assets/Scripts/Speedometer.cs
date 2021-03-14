using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{

    public RectTransform arrow;
    public TextMeshProUGUI text;

    public float range = 135;

    public float speedMultiplier=100;
    public float arrowMultiplier = 200;
    [Range(0,1)]
    public float arrowSpeed = 0.3f;

    private static Speedometer instance; 
    void Awake()
    {
        instance = this;
    }
    static public void SetSpeed(float speed)
    {
        instance.SetSpeedLocal(speed);
    }
    private float targetRotation;
    public void SetSpeedLocal(float speed) {
        text.SetText(Mathf.RoundToInt(speed*speedMultiplier).ToString());
        if (range - (speed * arrowMultiplier) > -range)
        {
            targetRotation =  range - (speed * arrowMultiplier);
        }
        else {
            targetRotation = -range-(15*Mathf.PerlinNoise(0,Time.realtimeSinceStartup*6));
        }
        arrow.localRotation = Quaternion.Lerp( arrow.localRotation,Quaternion.Euler(0, 0, targetRotation),arrowSpeed);
    }
}
