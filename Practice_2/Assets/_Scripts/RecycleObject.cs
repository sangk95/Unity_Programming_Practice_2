using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class RecycleObject : MonoBehaviour
{
    protected float lifetime = 0.5f;
    public Action<RecycleObject> Destroyed;
    public virtual void Activate(Vector3 startPosition)
    {
        transform.position = startPosition;
        StartCoroutine(DestroyDelay());
    }
    IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(lifetime);
        Destroyed?.Invoke(this);
    }
}
