using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UnitFactory 
{
    Unit[] prefab;
    int defaultPoolSize;
    UnitGenerator generator = new UnitGenerator();

    public UnitFactory(Unit[] prefab, int defaultPoolSize = 5)
    {
        this.prefab = prefab;
        this.defaultPoolSize = defaultPoolSize;
        Debug.Assert(this.prefab!=null, "Prefab is null");
    }
    public Unit GetUnit(string unit)
    {
        Unit returnUnit = generator.Get(prefab, unit, defaultPoolSize);

        return returnUnit;
    }
    public void Restore(Unit obj, string unit)
    {
        generator.Restore(obj, unit);
    }
}