using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UnitFactory 
{
    Unit[] prefab;
    int defaultPoolSize;
    
    EnemySpawner enemySpawner;
    UnitGenerator generator = new UnitGenerator();

    public UnitFactory(EnemySpawner enemySpawner, Unit[] prefab, int defaultPoolSize = 5)
    {
        this.enemySpawner = enemySpawner;
        this.prefab = prefab;
        this.defaultPoolSize = defaultPoolSize;
        Debug.Assert(this.prefab!=null, "Prefab is null");
    }
    public Unit GetUnit(string unit)
    {
        Unit returnUnit = generator.Get(prefab, unit, defaultPoolSize);
        foreach(var data in enemySpawner.GetEnemyStatData)
        {
            if(data.Name == unit)
            {
                returnUnit.Initialize(int.Parse(data.HP), int.Parse(data.ATK), float.Parse(data.Speed));
                break;
            }
        }
        return returnUnit;
    }
    public void Restore(Unit obj, string unit)
    {
        generator.Restore(obj, unit);
    }
}