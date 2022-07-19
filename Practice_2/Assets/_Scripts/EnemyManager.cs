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
    public Action<bool> MovingToNextWave;
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
        MovingToNextWave?.Invoke(false);
    }
    IEnumerator AutoSpawnEnemy()
    {
        while(true) 
        {
            if(currentEnemyCount >= waveEnemyCount && enemies.Count == 0)
            {
                currentWave++;
                if(currentWave >= maxWave)
                    yield break;
                MovingToNextWave?.Invoke(true);
                yield return new WaitForSeconds(waveInterval);
                MovingToNextWave?.Invoke(false);
                currentEnemyCount = 0;
            }
            else if(currentEnemyCount < waveEnemyCount)
                yield return new WaitForSeconds(enemySpawnInterval); 
            else
                yield return null;
            if(currentEnemyCount == 0 && currentWave != 0)
            {
                SpawnEnemy();
                NextStage?.Invoke();
            }
            else if(currentEnemyCount < waveEnemyCount)
                SpawnEnemy();
        }
    }
    public void resetActivate()
    {
        StartCoroutine(Reset());
    }
    IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.3f);
        foreach(var enemy in enemies)
        {
            enemy.ResetRotation(player.GetPosition);
        }
    }
    void SpawnEnemy()
    {
        Debug.Assert(this.unitGenerator != null, "enemy factory is null!");
        Debug.Assert(this.player != null, "player is null!");
        Unit unit = null;
        switch(currentWave)
        {
            case 0:
                unit = unitGenerator.GetUnit("Enemy_A");
                break;
            case 1:
                unit = unitGenerator.GetUnit("Enemy_B");
                break;
            case 2:
                unit = unitGenerator.GetUnit("Enemy_C");
                break;
        }
        unit.Activate(GetEnemySpawnPosition(), player.GetPosition);

        unit.Destroyed += this.OnEnemyDestroyed;
        enemies.Add(unit);
        currentEnemyCount++;
    }
    void OnEnemyDestroyed(Unit unit) 
    {
        unit.Destroyed -= this.OnEnemyDestroyed;
        int index = enemies.IndexOf(unit);
        enemies.RemoveAt(index); 
        unitGenerator.Restore(unit, unit.name);
        EnemyDestroyed?.Invoke();
        if (currentWave == maxWave-1 && enemies.Count == 0)
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

    public void Attacked(int damage, Unit unit)
    {
        int index = enemies.IndexOf(unit);
        enemies[index].Attacked(damage);
    }


    public void OnGameEnded(bool isVictory, int HeartCount)
    {
        if(enemies.Count == 0)
            return;
        
        foreach(var unit in enemies)
        {
            unitGenerator.Restore(unit, unit.name);
        }
    }
}
