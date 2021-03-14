using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBar : MonoBehaviour
{
    public Transform upgradeBar;
    public GameObject upgradeBarLine;
    public Button addPointButton;

    public Color activeColor;
    public Color passiveColor;

    public TextMeshProUGUI statName;
    public TextMeshProUGUI costText;

    private Image[] images;

    public void SetValue(string statName,int value, int maxValue, int cost) {
        if (images == null || images.Length != maxValue) {
            if (images != null)
            {
                foreach (Image item in images)
                {
                    if (item.gameObject != upgradeBarLine)
                    {
                        Destroy(item.gameObject);
                    }
                }
            }
            images = new Image[maxValue];
            images[0] = upgradeBarLine.GetComponent<Image>();
            for (int i = 0; i < maxValue-1; i++)
            {
                images[i+1]=Instantiate(upgradeBarLine,upgradeBar).GetComponent<Image>();
            }
        }

        for (int i = 0; i < maxValue; i++)
        {
            if (i < value)
            {
                images[i].color = activeColor;
            }
            else {
                images[i].color = passiveColor;
            }
        }

        costText.SetText(value < maxValue?cost.ToString():"");
        this.statName.SetText(statName);

        addPointButton.interactable = cost <= Menu.Currency && value<maxValue;
    }

    public void TryUpgrade() {
        UpgradeSystem.instance.Upgrade(this);
    }
}
