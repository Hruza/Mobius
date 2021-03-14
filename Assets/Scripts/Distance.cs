using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Distance : MonoBehaviour
{

    static public float Value {
        set {
            instance.text.SetText(Mathf.RoundToInt(value).ToString());
        }
    }

    public TextMeshProUGUI text;

    static private Distance instance;
    private void Awake()
    {
        instance = this;
    }
}
