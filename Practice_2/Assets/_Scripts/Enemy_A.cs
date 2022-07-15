using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_A :  Unit
{
    void Start()
    {
        moveSpeed = 2f;
        Hp = 1;
    }
    public override void Attack()
    {
        Debug.Log("Attack");
    }
    public override void DestroySelf()
    {
        isActivated = false;
        Destroyed?.Invoke(this);
        Hp=1;
    }
    void Update()
    {
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }
    
}
