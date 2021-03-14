using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fuel : MonoBehaviour
{
    public static float Value {
        set {
            value = Mathf.Clamp01(value);
            instance.SetArrow(value);
        }
    }

    static private Fuel instance;

    public static float maxFuel = 20;
    public static float fuel = 20;

    public RectTransform arrow;
    public Image background;
    public float range = 135;
    [Range(0, 1)]
    public float arrowSpeed = 0.3f;

    private float targetRotation;

    Color col;

    private void SetArrow(float value) {
        targetRotation = range-(2*range*value);
        arrow.localRotation = Quaternion.Lerp(arrow.localRotation, Quaternion.Euler(0, 0, targetRotation), arrowSpeed);
        background.fillAmount = Mathf.Lerp(background.fillAmount,(value * 0.75f) + 0.125f,arrowSpeed);
        col=Color.HSVToRGB(value*0.35f,1,1);
        col.a = 0.5f;
        background.color = col;
    }

    public static bool HasFuel {
        get {
            return fuel > 0;
        }
    }

    public static void UseFuel(float amount) {
        fuel = Mathf.Clamp(fuel - amount,0,maxFuel);
        Value = fuel / maxFuel;
    }

    public static void AddFuel(float amount) {
        fuel = Mathf.Clamp(fuel + amount, 0, maxFuel);
        Value = fuel / maxFuel;
    }

    public void Awake()
    {
        instance = this;
    }
}
