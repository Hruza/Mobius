using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "ScriptableObjects/World", order = 1)]
public class World : ScriptableObject
{
    public Color loopColor;
    public float meanCoinSpacing=50;

    public GameObject[] obstacles;
    public float meanObstacleSpacing=50;

    public GameObject[] boosts;
    public float meanBoostSpacing=150;

    public int loopLength = 10;
}
