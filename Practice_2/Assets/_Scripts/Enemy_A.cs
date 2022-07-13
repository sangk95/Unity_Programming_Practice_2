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
    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        box.isTrigger = true; 
    }
    void Start()
    {
        Hp = 2;
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
        Destroyed?.Invoke(this);
        Destroy(this.gameObject);
    }
    void Update()
    {
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }
    
}
