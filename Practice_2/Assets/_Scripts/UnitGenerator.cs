using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGenerator
{
    Unit[] prefab;
    public UnitGenerator(Unit[] prefab)
    {
        this.prefab = prefab;
        
        Debug.Assert(this.prefab!=null, "Prefab is null");
    }
    public Unit CreatUnit(string unit)
    {
        UnitFactory factory = new UnitFactory();
        Unit returnUnit = factory.CreateUnit(prefab, unit);

        return returnUnit;
    }
}
