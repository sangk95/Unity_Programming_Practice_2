using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitGenerator : MonoBehaviour
{
    public List<Unit> units = new List<Unit>();
    public List<Unit> getUnits()
    {
        return units;
    }
    public abstract void CreateUnits();
}
