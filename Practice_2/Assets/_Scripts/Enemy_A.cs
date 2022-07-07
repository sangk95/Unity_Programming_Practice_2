using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class Enemy_A :  Unit
{
    [SerializeField]
    float moveSpeed = 2f; 
    BoxCollider2D box;
    Rigidbody2D body;
    Unit prefab;/*
    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        box.isTrigger = true; 
    }*/
    public Enemy_A(Unit prefab)
    {
        this.prefab = prefab;
        type = UnitType.enemy_A;
        Hp = 2;
        
        Debug.Assert(prefab != null, "Unit Prefab is null");
    }
    public override void Attack()
    {
        Debug.Log("Attack");
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
            Hp--;
            if(Hp==0)
                DestroySelf();
            return;
        }
    }
    void DestroySelf()
    {
        isActivated = false;
        Destroyed?.Invoke(this);
    }/*
    void Update()
    {
        if(!isActivated)
            return;
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }*/
    
}
