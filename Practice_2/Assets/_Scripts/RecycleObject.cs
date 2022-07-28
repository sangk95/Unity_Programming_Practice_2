using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class RecycleObject : MonoBehaviour
{
    protected float lifetime = 0.3f;
    public Action<RecycleObject> Destroyed;
    public virtual void Activate(Vector3 startPosition, Vector3 vec)
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.LookRotation(transform.forward, vec);
        StartCoroutine(DestroyDelay());
    }
    IEnumerator DestroyDelay()
    {
        while(lifetime > 0)
        {
            lifetime -= Time.deltaTime;
            this.transform.position += transform.up * 1.5f * Time.deltaTime;
            yield return null;
        }
        lifetime = 0.3f;
        Destroyed?.Invoke(this);
    }
}
