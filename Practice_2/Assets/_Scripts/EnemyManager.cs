using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EnemyManager : MonoBehaviour
{
    Factory enemyFactory;
    PlayerController player;
    int maxEnemyCount = 20;
    int currentEnemyCount;
    float enemySpawnInterval = 0.5f;
    bool isInitialized = false;
    public Action EnemyDestroyed;
    public Action AllEnemyDestroyed; 
    List<RecycleObject> enemies = new List<RecycleObject>();

    public Vector3 GetFirstEnemy{get{return enemies[0].transform.position;}}
    public void Initialize(Factory enemyFactory, PlayerController player, int maxEnemyCount, float enemySpawnInterval)
    {
        if(isInitialized)
            return;
        this.enemyFactory = enemyFactory;
        this.player = player;
        this.maxEnemyCount = maxEnemyCount;
        this.enemySpawnInterval = enemySpawnInterval;
        
        Debug.Assert(this.enemyFactory != null, "missile factory is null!");
        Debug.Assert(this.player != null, "building manager is null!");

        isInitialized = true;
    }
    public void Gamestart()
    {
        currentEnemyCount = 0;
        StartCoroutine(AutoSpawnEnemy());
    }
    IEnumerator AutoSpawnEnemy()
    {
        while(currentEnemyCount < maxEnemyCount) 
        {
            yield return new WaitForSeconds(enemySpawnInterval); 

            SpawnEnemy();
        }
    }
    void SpawnEnemy()
    {
        Debug.Assert(this.enemyFactory != null, "enemy factory is null!");
        Debug.Assert(this.player != null, "player is null!");

        RecycleObject enemy = enemyFactory.Get();
        enemy.Activate(GetEnemySpawnPosition(), player.GetPosition); 

        enemy.Destroyed += this.OnEnemyDestroyed;
        enemies.Add(enemy);
        currentEnemyCount++;
    }
    void OnEnemyDestroyed(RecycleObject enemy) 
    {
        enemy.Destroyed -= this.OnEnemyDestroyed;
        int index = enemies.IndexOf(enemy);
        enemies.RemoveAt(index); 
        enemyFactory.Restore(enemy);
        if (currentEnemyCount == maxEnemyCount && enemies.Count == 0)
        {
            AllEnemyDestroyed?.Invoke();
        }
    }
    Vector3 GetEnemySpawnPosition() 
    {
        Vector3 spawnPosition = Vector3.zero;
        spawnPosition.x = UnityEngine.Random.Range(0f, 1f);
        spawnPosition.y = 1f; 

        spawnPosition = Camera.main.ViewportToWorldPoint(spawnPosition);
        
        spawnPosition.z = 0f;
        return spawnPosition;
    }

}
