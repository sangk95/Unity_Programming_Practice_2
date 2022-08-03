using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent(typeof(BoxCollider2D))]
public class RecycleObject : MonoBehaviour
{
    BoxCollider2D box;
    protected float lifetime = 0.3f;
    public Action<RecycleObject> Destroyed;
    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
    }
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
            yield return null;
        }
        lifetime = 0.3f;
        Destroyed?.Invoke(this);
    }
}
