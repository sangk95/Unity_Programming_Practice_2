using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Bullet : RecycleObject
{
    BoxCollider2D box;
    [SerializeField]
    float speed = 10f;
    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
    }

    bool isArrivedToTarget()
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        return distance < 0.1f;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.GetComponent<Enemy>() != null)
        {
            Destroyed?.Invoke(this);
            return;
        }
    }

    void Update()
    {
        if(!isActivated)
            return;
        transform.position += transform.up * speed * Time.deltaTime;
        if(isArrivedToTarget())
        {
            isActivated = false;
            Destroyed?.Invoke(this);
        }
    }
}
