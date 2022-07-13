using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGenerator
{
    Unit[] prefab;
    int defaultPoolSize;
    UnitFactory factory = new UnitFactory();

    public UnitGenerator(Unit[] prefab, int defaultPoolSize = 5)
    {
        this.prefab = prefab;
        this.defaultPoolSize = defaultPoolSize;
        Debug.Assert(this.prefab!=null, "Prefab is null");
    }
    public Unit GetUnit(string unit)
    {
        Unit returnUnit = factory.Get(prefab, unit, defaultPoolSize);

        return returnUnit;
    }
    public void Restore(Unit obj, string unit)
    {
        factory.Restore(obj, unit);
    }
}
