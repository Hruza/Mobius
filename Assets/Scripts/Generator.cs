using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    
    public World[] worlds;
    public static World CurrentWorld {
        get {
            return instance.worlds[Mathf.Min(instance.currentWorldIndex, instance.worlds.Length-1)];
        }
    }

    private int currentWorldIndex;

    public float zOffset=0.1f;

    public GameObject gateway;
    public GameObject coin;

    public static Generator instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public static void NextWorld() {
        instance.currentWorldIndex ++;
        instance.ChangeWorld();
    }

    public static void ResetWorld() {
        instance.currentWorldIndex = 0;
        instance.nextWorldMilestone = 0;
        instance.ChangeWorld();
    }

    private void ChangeWorld() {
        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }
        Mobius.ChangeColor(CurrentWorld.loopColor);
        nextWorldMilestone += CurrentWorld.loopLength;
        gateSpawned = false;

        instance.nextCoinMilestone = PlayerController.distance+NewDistance(CurrentWorld.meanCoinSpacing);
        instance.nextObstacleMilestone = PlayerController.distance + NewDistance(CurrentWorld.meanObstacleSpacing);
        instance.nextBoostMilestone = PlayerController.distance + NewDistance(CurrentWorld.meanBoostSpacing);
    }

    int nextWorldMilestone=0;
    float nextCoinMilestone=0;
    float nextObstacleMilestone=0;
    float nextBoostMilestone=0;

    float distance;

    bool gateSpawned;

    private void Update()
    {
        if (PlayerController.distance > 0)
        {
            if (!gateSpawned && PlayerController.LoopCount > nextWorldMilestone - 0.4f)
            {
                GameObject created = Instantiate(gateway);
                MobiusTransform tr = created.GetComponent<MobiusTransform>();
                if (tr == null)
                {
                    tr = created.AddComponent<MobiusTransform>();
                }
                tr.SetPosition(0, 0, Generator.instance.zOffset);
                gateSpawned = true;
            }

            distance = PlayerController.distance;

            if (distance > nextCoinMilestone - 100)
            {
                SpawnObject(coin, nextCoinMilestone);
                nextCoinMilestone += NewDistance(CurrentWorld.meanCoinSpacing);
            }

            if (distance > nextObstacleMilestone - 100)
            {
                SpawnObject(CurrentWorld.obstacles[Random.Range(0, CurrentWorld.obstacles.Length)], nextObstacleMilestone);
                nextObstacleMilestone += NewDistance(CurrentWorld.meanObstacleSpacing);
            }

            if (distance > nextBoostMilestone - 100)
            {
                SpawnObject(CurrentWorld.boosts[Random.Range(0, CurrentWorld.boosts.Length)], nextBoostMilestone);
                nextBoostMilestone += NewDistance(CurrentWorld.meanBoostSpacing);
            }
        }
    }

    private static float NewDistance(float meanValue) {
        return Random.Range(meanValue / 2, 3 * meanValue / 2);
    }

    static void SpawnObject(GameObject gameObject,float distance) {
        GameObject created = Instantiate(gameObject,instance.transform);
        MobiusTransform tr = created.GetComponent<MobiusTransform>();
        Obstacle obstacle = created.GetComponent<Obstacle>();
        float offset=0;
        if (obstacle != null) {
            obstacle.destroyPoint = distance + obstacle.destroyAfterDistance;
            offset = obstacle.edgeOffset;
        }
        if (tr == null) {
            tr=created.AddComponent<MobiusTransform>();
        }
        tr.SetPosition(distance/Mobius.instance.radius,Random.Range((-0.5f*Mobius.instance.width)+offset,(0.5f*Mobius.instance.width)-offset),Generator.instance.zOffset);
    }
}
