using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_A :  Unit
{
    void Start()
    {
        moveSpeed = 1f;
        Hp = 2;
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
    public override void Attack()
    {
        Debug.Log("Attack");
    }
    public override void DestroySelf()
    {
        isActivated = false;
        Destroyed?.Invoke(this);
        Hp=2;
    }
    void Update()
    {
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }
    
}
