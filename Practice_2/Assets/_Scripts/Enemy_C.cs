using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_C :  Unit
{
    void Start()
    {
        moveSpeed = 2f;
        Hp = 10;
        ATKDamage = 1;
    }
    public override void DestroySelf()
    {
        isActivated = false;
        Destroyed?.Invoke(this);
        Hp=10;
    }
    void Update()
    {
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }
    
}
