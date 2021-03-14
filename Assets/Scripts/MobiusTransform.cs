using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobiusTransform : MonoBehaviour
{
    private float fi;
    private float d;
    private float z=1;

    public float edgeOffset = 0.5f;

    public float Fi {
        get { return fi; }
        set{
            fi = value;
            transform.position = MobiusToReal(fi, d, z);
        }
    }
    public float D {
        get { return d; }
        set {
            d = Mathf.Clamp(value,-w/2+ edgeOffset, w/2- edgeOffset);
            transform.position = MobiusToReal(fi, d, z);
        }
    }
    public float Z
    {
        get { return z; }
        set {
            z = value;
            transform.position = MobiusToReal(fi, d, z);
        }
    }

    public void SetPosition(float fi, float d, float z) {
        Fi = fi;
        D = d;
        Z = z;
        transform.position = MobiusToReal(fi, d, z);
    }

    private Mobius mobius;
    private float r;
    private float w;

    private void Start()
    {
        mobius = Mobius.instance;
        w = Mobius.instance.width;
    }

    private Vector3 MobiusToReal(float fi,float d, float z) {
        if (mobius == null) {
            Start();
        }
        r = mobius.radius;
        Vector3 mobiusPoint = new Vector3( 
                                       (r + (d * (Mathf.Cos(fi)))) * Mathf.Cos(2 * fi),
                                       d * (Mathf.Sin(fi)),
                                       (r + (d * (Mathf.Cos(fi)))) * Mathf.Sin(2 * fi)
                                  );

        Vector3 normal=new Vector3( 
                                -(Mathf.Sin(fi)) * Mathf.Cos(2 * fi),
                                 (Mathf.Cos(fi)),
                                -(Mathf.Sin(fi)) * Mathf.Sin(2 * fi)
                           );
        transform.rotation = Quaternion.LookRotation(new Vector3(-mobiusPoint.z,0,mobiusPoint.x),normal);
        return mobiusPoint + (z * normal);
    }

 
}
