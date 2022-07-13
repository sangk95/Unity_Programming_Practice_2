using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EnemyManager : MonoBehaviour
{
    UnitGenerator unitGenerator;
    PlayerController player;
    int maxWave = 2;
    int currentEnemyCount=0;
    int waveEnemyCount = 5;
    int currentWave=0;
    float waveInterval = 5f;
    float enemySpawnInterval = 0.5f;
    bool isInitialized = false;
    public Action EnemyDestroyed;
    public Action AllEnemyDestroyed; 
    public Action NextStage;
    List<Unit> enemies = new List<Unit>();

    /*  --FireController-- It's not nearest
    public bool IsEnemyLeft{get{return enemies.Count>0;}}
    public Vector3 GetFirstEnemy{get{return enemies[0].transform.position;}}
    */
    public void Initialize(UnitGenerator unitGenerator, PlayerController player, int maxWave, int waveEnemyCount, float waveInterval, float enemySpawnInterval)
    {
        if(isInitialized)
            return;
        this.unitGenerator = unitGenerator;
        this.player = player;
        this.maxWave = maxWave;
        this.waveEnemyCount = waveEnemyCount;
        this.waveInterval = waveInterval;
        this.enemySpawnInterval = enemySpawnInterval;
        
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
        while(true) 
        {
            if(waveEnemyCount <= currentEnemyCount)
            {
                yield return new WaitForSeconds(waveInterval);
                currentEnemyCount = 0;
                currentWave++;
                if(currentWave >= maxWave)
                    break;
                SpawnEnemy();
                NextStage?.Invoke();
            }
            else
                yield return new WaitForSeconds(enemySpawnInterval); 

            SpawnEnemy();
        }
    }
    public void resetActivate(Vector3 position)
    {
        StartCoroutine(reset());
    }
    IEnumerator reset()
    {
        yield return new WaitForSeconds(0.3f);
        foreach(var enemy in enemies)
        {
            enemy.Activate(enemy.transform.position, player.GetPosition);
        }
    }
    void SpawnEnemy()
    {
        Debug.Assert(this.unitGenerator != null, "enemy factory is null!");
        Debug.Assert(this.player != null, "player is null!");
        Unit unit;
        if(currentWave==0)
            unit = unitGenerator.GetUnit("Enemy_A");
        else
            unit = unitGenerator.GetUnit("Enemy_B");
        unit.Activate(GetEnemySpawnPosition(), player.GetPosition);

        unit.Destroyed += this.OnEnemyDestroyed;
        enemies.Add(unit);
        currentEnemyCount++;
    }
    void OnEnemyDestroyed(Unit enemy) 
    {
        enemy.Destroyed -= this.OnEnemyDestroyed;
        int index = enemies.IndexOf(enemy);
        enemies.RemoveAt(index); 
        unitGenerator.Restore(enemy, enemy.name);
        if (currentWave == maxWave && enemies.Count == 0)
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
