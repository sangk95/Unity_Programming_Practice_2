using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory
{
    RecycleObject[] prefab;
    int defaultPoolSize;
    Generator generator = new Generator();

    public Factory(RecycleObject[] prefab, int defaultPoolSize = 5)
    {
        this.prefab = prefab;
        this.defaultPoolSize = defaultPoolSize;
        Debug.Assert(this.prefab!=null, "Prefab is null");
    }
    public RecycleObject GetObject(string unit)
    {
        RecycleObject returnUnit = generator.Get(prefab, unit, defaultPoolSize);

        return returnUnit;
    }
    public void Restore(RecycleObject obj, string unit)
    {
        generator.Restore(obj, unit);
    }
}
