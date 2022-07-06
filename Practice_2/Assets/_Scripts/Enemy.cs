using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class Enemy : RecycleObject
{
    BoxCollider2D box;
    Rigidbody2D body;
    [SerializeField]
    float moveSpeed = 2f; 
    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        box.isTrigger = true; 
    } 
    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.GetComponent<PlayerController>() != null)
        {
            DestroySelf();
            return;
        }
        if(other.GetComponent<Bullet>() != null)
        {
            DestroySelf();
            return;
        }
    }
    
    void DestroySelf()
    {
        isActivated = false;
        Destroyed?.Invoke(this);
    }
    void Update()
    {
        if(!isActivated)
            return;
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }
}
