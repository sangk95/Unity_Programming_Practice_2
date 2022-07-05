using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : RecycleObject
{
    [SerializeField]
    float speed = 10f;

    bool isArrivedToTarget()
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        return distance < 0.1f;
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
