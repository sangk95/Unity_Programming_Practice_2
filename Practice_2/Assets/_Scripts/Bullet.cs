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

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Enemy"))
        {
            isActivated = false;
            Unit unit = other.GetComponent<Unit>();
            Destroyed?.Invoke(this, unit);
            return;
        }
    }

    void Update()
    {
        if(!isActivated)
            return;
        transform.position += transform.up * speed * Time.deltaTime;
    }
}
