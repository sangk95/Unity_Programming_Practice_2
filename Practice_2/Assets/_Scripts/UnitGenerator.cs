using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class UnitGenerator
{
    Dictionary<string, List<Unit>> pool = new Dictionary<string, List<Unit>>();
    int defaultPoolSize;

    ///////Get을 추상메소드로 선언한 후 자식클래스에서 각자 어떤 객체를 생성할지 결정한다면 팩토리메소드패턴이다.
    ///////자식클래스에서 생성할 객체의 세부정보를 가지고 있는 인터페이스를 따로 만들어 관리한다면 추상팩토리패턴이다.
    // 현재는 심플팩토리 
    public Unit Get(Unit[] prefab, string unit, int defaultPoolSize)
    {
        this.defaultPoolSize = defaultPoolSize;
        Unit returnUnit = null;
        switch(unit)
        {
            case "Enemy_A":
                returnUnit =  CreateUnit(prefab[0], unit);
                break;
            case "Enemy_B":
                returnUnit = CreateUnit(prefab[1], unit);
                break;
        }
        return returnUnit;
    }
    void CreatePool(Unit prefab, string unit)
    {
        List<Unit> temp = new List<Unit>();
        for(int i=0 ; i<defaultPoolSize ; i++)
        {
            Unit obj = GameObject.Instantiate(prefab) as Unit;
            obj.gameObject.SetActive(false);
            temp.Add(obj);
        }
        pool.Add(unit, temp);
    }

    public Unit CreateUnit(Unit prefab, string unit)
    {
        if(!pool.ContainsKey(unit) || pool[unit].Count == 0)
        {
            CreatePool(prefab, unit);
        }
        int lastIndex = pool[unit].Count-1;
        Unit obj = pool[unit][lastIndex];
        pool[unit].RemoveAt(lastIndex);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Restore(Unit obj, string unit)
    {
        string name = unit.Replace("(Clone)","");
        Debug.Assert(obj != null, "Null object to be returned!");
        obj.gameObject.SetActive(false);
        pool[name].Add(obj);
    }
}