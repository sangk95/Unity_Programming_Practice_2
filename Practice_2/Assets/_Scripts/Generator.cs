using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Generator
{
    Dictionary<string, List<RecycleObject>> pool = new Dictionary<string, List<RecycleObject>>();
    int defaultPoolSize;

    public RecycleObject Get(RecycleObject[] prefab, string name, int defaultPoolSize)
    {
        this.defaultPoolSize = defaultPoolSize;
        RecycleObject returnRecycleObject = null;
        switch(name)
        {
            case "Normal_1":
                returnRecycleObject =  CreateRecycleObject(prefab[0], name);
                break;
            case "Skill_1":
                returnRecycleObject = CreateRecycleObject(prefab[1], name);
                break;
            default:
                break;
        }
        return returnRecycleObject;
    }
    void CreatePool(RecycleObject prefab, string name)
    {
        List<RecycleObject> temp = new List<RecycleObject>();
        for(int i=0 ; i<defaultPoolSize ; i++)
        {
            RecycleObject obj = GameObject.Instantiate(prefab) as RecycleObject;
            obj.gameObject.SetActive(false);
            temp.Add(obj);
        }
        pool.Add(name, temp);
    }

    public RecycleObject CreateRecycleObject(RecycleObject prefab, string name)
    {
        if(!pool.ContainsKey(name) || pool[name].Count == 0)
        {
            CreatePool(prefab, name);
        }
        int lastIndex = pool[name].Count-1;
        RecycleObject obj = pool[name][lastIndex];
        pool[name].RemoveAt(lastIndex);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Restore(RecycleObject obj, string name)
    {
        string name_ = name.Replace("(Clone)","");
        Debug.Assert(obj != null, "Null object to be returned!");
        obj.gameObject.SetActive(false);
        pool[name_].Add(obj);
    }
}
