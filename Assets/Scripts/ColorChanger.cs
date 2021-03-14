using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
    public Color[] colors;

    public Material material;

    public string keyWord = "col";

    private Image img;

    private void Start()
    {
        img = GetComponent<Image>();
        if (PlayerPrefs.HasKey(keyWord)) {
            index = PlayerPrefs.GetInt(keyWord)%colors.Length;
            img.color = colors[index];
        }
        else {
            img.color = colors[0];
        }
        img.color = colors[index];
        material.color = colors[index];
    }

    int index = 0;
    public void Clicked()
    {
        index++;
        index = index % colors.Length;

        img.color = colors[index];
        material.color = colors[index];

        PlayerPrefs.SetInt(keyWord,index);
    }
}
