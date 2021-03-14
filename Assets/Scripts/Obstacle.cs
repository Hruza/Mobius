using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType { coin, fuel, slowDown,fuelDrain,gateway };

public class Obstacle : MonoBehaviour
{
    public ObstacleType type;
    public float value;
    public float destroyAfterDistance = 400;
    [HideInInspector]
    public float destroyPoint=0;
    public GameObject OnDestroyParticles;

    public float edgeOffset = 0.5f;

    public static List<GameObject> instances;

    private void Start()
    {
        if (instances == null) {
            instances = new List<GameObject>();
        }
        instances.Add(this.gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision");
        if (other.CompareTag("Player")) {
            switch (type)
            {
                case ObstacleType.coin:
                    Menu.Currency+=Mathf.RoundToInt(value);
                    break;
                case ObstacleType.fuel:
                    Fuel.AddFuel(value);
                    break;
                case ObstacleType.fuelDrain:
                    Fuel.UseFuel(value);
                    break;
                case ObstacleType.slowDown:
                    CameraShake.DoShake();
                    PlayerController.SlowDown(value);
                    break;
                case ObstacleType.gateway:
                    PlayerController.SlowDown(value);
                    Generator.NextWorld();
                    break;
                default:
                    break;
            }
            if (OnDestroyParticles != null) {
                Destroy(Instantiate(OnDestroyParticles,transform.position+transform.up,transform.rotation),3);
            }
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (destroyPoint > 0 && PlayerController.distance>destroyPoint) {
            Destroy(gameObject);
        }
    }
}
