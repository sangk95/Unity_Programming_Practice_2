using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class UnitFactory 
{
    public Unit CreateUnit(Unit[] prefab, string unit)
    {
        Unit returnUnit = null;
        switch(unit)
        {
            case "A":
                returnUnit = GameObject.Instantiate(prefab[0]) as Unit;
                break;
            case "B":
                returnUnit = GameObject.Instantiate(prefab[1]) as Unit;
                break;
        }
        return returnUnit;
    }
}