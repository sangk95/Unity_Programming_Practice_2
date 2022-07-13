using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public abstract class Unit : Objects
{
    Unit prefab;
    protected int Hp;
    protected float moveSpeed; 
    BoxCollider2D box;
    Rigidbody2D body;
    public abstract void Attack();
    public Action<Unit> Destroyed;
    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        box.isTrigger = true; 
    }
    public abstract void DestroySelf();
}
