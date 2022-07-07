using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum UnitType
{
    enemy_A,
    enemy_B,
    enemy_C
}
public abstract class Unit
{
    protected UnitType type;
    protected int Hp;
    protected bool isActivated = false;
    protected Vector3 targetPosition;
    
    public Action<Unit> Destroyed;
    public abstract void Attack();
    
}
