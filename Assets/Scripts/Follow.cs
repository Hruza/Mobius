using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    [Range(0, 1)]
    public float followSpeed=0.8f;
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position,target.position,followSpeed);
        transform.rotation = target.rotation;
    }
}
