using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    private bool isRotating;
    private Vector3 startingRotation;
    private Vector2 mouseReference;
    private Quaternion target;
    private Vector2 mouseOffset;
    private Vector3 axis;
    private Quaternion newRotation;

    private float sensitivity = 30f;

    private Quaternion parentRotation;
    void Start()
    {
        parentRotation = transform.parent.rotation;
    }

    void Update()
    {
        if (isRotating)
        {
            mouseOffset = ((Vector2)Input.mousePosition - mouseReference);

            newRotation = Quaternion.AngleAxis(mouseOffset.magnitude*Time.deltaTime*sensitivity,parentRotation*new Vector3(-mouseOffset.y,-mouseOffset.x,0));

            Debug.DrawRay(transform.position, parentRotation * new Vector3(-mouseOffset.y, mouseOffset.x, 0)*100,Color.red);

            target = newRotation*target;
            
            mouseReference = Input.mousePosition;

            transform.rotation = Quaternion.Lerp(transform.rotation,target,0.4f);
        }
    }

    void OnMouseDown()
    {
        isRotating = true;
        mouseReference = Input.mousePosition;
        target = transform.rotation;
    }

    void OnMouseUp()
    {
        // rotating flag
        isRotating = false;
    }
}
