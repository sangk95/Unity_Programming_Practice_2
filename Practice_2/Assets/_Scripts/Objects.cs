using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Objects : MonoBehaviour
{
    protected Vector3 targetPosition;    
    protected bool isActivated = false;
    public void Activate(Vector3 startPosition, Vector3 targetPosition)
    {
        transform.position = startPosition;
        this.targetPosition = targetPosition;
        Vector3 dir = (targetPosition - startPosition).normalized;
        transform.rotation = Quaternion.LookRotation(transform.forward, dir);
        isActivated = true;
    }
}
