using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Objects : MonoBehaviour
{
    protected Vector3 targetPosition;    
    protected bool isActivated = false;
    public virtual void Activate(Vector3 startPosition)
    {
        transform.position = startPosition;
        isActivated = true;
    }
    public virtual void Activate(Vector3 startPosition, Vector3 targetPosition)
    {
        transform.position = startPosition;
        this.targetPosition = targetPosition;
        if(!isActivated)
            transform.rotation = Quaternion.Euler(0,0,180);
        Vector3 dir = (targetPosition - startPosition).normalized;
        StartCoroutine(slerpRotation(dir));
        isActivated = true;
    }
    public void ResetRotation(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        Vector3 dir = (targetPosition - transform.position).normalized;
        StartCoroutine(slerpRotation(dir));
    }

    IEnumerator slerpRotation(Vector3 dir)
    {
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, dir);
        while(Quaternion.Euler(transform.rotation.eulerAngles) != targetRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime*2);
            yield return null;
        }
    }
}
