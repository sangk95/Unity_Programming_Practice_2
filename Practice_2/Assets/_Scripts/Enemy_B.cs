using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_B :  Unit
{
    void Start()
    {
        moveSpeed = 1f;
        Hp = 2;
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
