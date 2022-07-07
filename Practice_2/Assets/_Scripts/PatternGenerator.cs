using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class PatternAGenerator : UnitGenerator
{
    Unit prefab;
    public PatternAGenerator(Unit prefab)
    {
        this.prefab = prefab;
    }
    public override void CreateUnits()
    {
        units.Add(new Enemy_A(prefab));
        units.Add(new Enemy_A(prefab));
        units.Add(new Enemy_A(prefab));
    }
}
 
class PatternBGenerator : UnitGenerator
{
    Unit prefab;
    public PatternBGenerator(Unit prefab)
    {
        this.prefab = prefab;
    }
    public override void CreateUnits()
    {
        units.Add(new Enemy_A(prefab));
        units.Add(new Enemy_A(prefab));
        units.Add(new Enemy_A(prefab));
    }
}
