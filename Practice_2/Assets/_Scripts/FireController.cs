using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class FireController
{
    public Action<Vector3> Fire;
    EnemyManager enemyManager;
    PlayerController player;

    public FireController(EnemyManager enemyManager, PlayerController player)
    {
        this.enemyManager = enemyManager;
        this.player = player;
    }
    
    public void NearestEnemy()
    {
        var objects = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        var neareastObject = objects.OrderBy(obj => {return Vector3.Distance(player.transform.position, obj.transform.position);}).FirstOrDefault();
        if(neareastObject == null)
            return;
        Fire?.Invoke(neareastObject.transform.position);
        
        /*
        if(!enemyManager.IsEnemyLeft)
            return;
        Fire?.Invoke(enemyManager.GetFirstEnemy);
        */
    }

}
