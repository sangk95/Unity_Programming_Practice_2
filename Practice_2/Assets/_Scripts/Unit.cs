using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Unit : MonoBehaviour
{
    protected int Hp;
    protected Vector3 targetPosition;
    
    public Action<Unit> Destroyed;
    Unit prefab;
    public abstract void Attack();
    public virtual void Activate(Vector3 startPosition, Vector3 targetPosition)
    {
        transform.position = startPosition;
        this.targetPosition = targetPosition;
        Vector3 dir = (targetPosition - startPosition).normalized;
        transform.rotation = Quaternion.LookRotation(transform.forward, dir);
    }
    
}
