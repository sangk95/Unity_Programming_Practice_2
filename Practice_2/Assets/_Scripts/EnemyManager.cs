using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EnemyManager : MonoBehaviour
{
    UnitGenerator[] unitGenerators = null;
    Factory enemyFactory;
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
    List<RecycleObject> enemies = new List<RecycleObject>();
    Unit prefab;

    /*  --FireController-- It's not nearest
    public bool IsEnemyLeft{get{return enemies.Count>0;}}
    public Vector3 GetFirstEnemy{get{return enemies[0].transform.position;}}
    */
    public void Initialize(Unit prefab,Factory enemyFactory, PlayerController player, int maxWave, int waveEnemyCount, float waveInterval, float enemySpawnInterval)
    {
        if(isInitialized)
            return;
        this.prefab = prefab;
        this.enemyFactory = enemyFactory;
        this.player = player;
        this.maxWave = maxWave;
        this.waveEnemyCount = waveEnemyCount;
        this.waveInterval = waveInterval;
        this.enemySpawnInterval = enemySpawnInterval;
        
        Debug.Assert(this.enemyFactory != null, "missile factory is null!");
        Debug.Assert(this.player != null, "building manager is null!");

        isInitialized = true;
    }
    void Start()
    {
        unitGenerators = new UnitGenerator[maxWave];
        for(int i=0 ; i<maxWave ; i++)
        {
            unitGenerators[i] = new PatternAGenerator(prefab);
        }
    }
    public void Gamestart()
    {
        currentEnemyCount = 0;
        StartCoroutine(AutoSpawnEnemy());
    }
    IEnumerator AutoSpawnEnemy()
    {/*
        unitGenerators[0].CreateUnits();
        List<Unit> units = unitGenerators[0].getUnits();
        foreach(Unit unit in units)
        {
            unit.transform.position = GetEnemySpawnPosition();
            unit.Attack();
            yield return null;
        }*/
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
