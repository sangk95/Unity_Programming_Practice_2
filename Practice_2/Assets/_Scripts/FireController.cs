using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class FireController
{
    public Action<Vector3> Fire;
    EnemyManager enemyManager;

    public FireController(EnemyManager enemyManager)
    {
        this.enemyManager = enemyManager;
    }
    
    public void NearestEnemy()
    {
        Fire?.Invoke(enemyManager.GetFirstEnemy);
        Debug.Log("Find!!!!!");
    }

}
