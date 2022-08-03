using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EnemyManager : MonoBehaviour
{
    UnitFactory unitFactory;
    PlayerController player;
    EnemySpawner enemySpawner;
    int maxWave = 3;
    int currentEnemyCount=0;
    int waveEnemyCount = 0;
    int currentWave=0;
    float waveInterval = 5f;
    float enemySpawnInterval = 0.5f;
    bool isInitialized = false;
    bool isSpawning = false;
    public Action EnemyDestroyed;
    public Action AllEnemyDestroyed; 
    public Action NextStage;
    public Action WaveEnd;
    public Action<bool> MovingToNextWave;
    public Action<int> WaveStarted;
    public Action<int> AttackPlayer;
    List<Unit> enemies = new List<Unit>();
    Dictionary<string, int> EnemyTypeCount = new Dictionary<string, int>();

    /*  --FireController-- It's not nearest
    public bool IsEnemyLeft{get{return enemies.Count>0;}}
    public Vector3 GetFirstEnemy{get{return enemies[0].transform.position;}}
    */
    public void Initialize(EnemySpawner enemySpawner, UnitFactory unitFactory, PlayerController player)
    {
        if(isInitialized)
            return;
        this.enemySpawner = enemySpawner;
        this.unitFactory = unitFactory;
        this.player = player;
        
        Debug.Assert(this.player != null, "building manager is null!");

        isInitialized = true;
    }
    int CurWaveEnemyCount()
    {
        int enemyCount=0;
        EnemyTypeCount = enemySpawner.GetEnemyTypeCount(currentWave+1);
        foreach(var data in EnemyTypeCount)
        {
            enemyCount += data.Value;
        }
        return enemyCount;
    }
    public void Gamestart()
    {
        maxWave = enemySpawner.GetMaxWave;
        waveEnemyCount = CurWaveEnemyCount();
        currentEnemyCount = 0;
        StartCoroutine(AutoSpawnEnemy());
        MovingToNextWave?.Invoke(false);
        WaveStarted?.Invoke(currentWave+1);
    }
    IEnumerator AutoSpawnEnemy()
    {
        while(true) 
        {
            if(!isSpawning)
                StartCoroutine(SpawnEnemy());
            else
            {
                if(enemies.Count == 0)
                {
                    WaveEnd?.Invoke();
                    currentWave++;
                    if(currentWave >= maxWave)
                        yield break;
                    waveEnemyCount = CurWaveEnemyCount();
                    MovingToNextWave?.Invoke(true);
                    yield return new WaitForSeconds(waveInterval);
                    MovingToNextWave?.Invoke(false);
                    WaveStarted?.Invoke(currentWave+1);
                    currentEnemyCount = 0;
                    NextStage?.Invoke();
                    isSpawning = false;
                }
                else
                    yield return null;
            }
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
    IEnumerator SpawnEnemy()
    {
        Debug.Assert(this.unitFactory != null, "enemy factory is null!");
        Debug.Assert(this.player != null, "player is null!");
        isSpawning = true;
        Unit unit = null;

        foreach(var data in EnemyTypeCount)
        {
            for(int i=0 ; i<data.Value ; i++)
            {
                unit = unitFactory.GetUnit(data.Key);
                
                unit.Activate(GetEnemySpawnPosition(), player.GetPosition);

                unit.Destroyed += this.OnEnemyDestroyed;
                unit.AttackedBySword += this.OnEnemyAttacked;
                unit.AttackPlayer += this.OnAttackPlayer;
                enemies.Add(unit);
                currentEnemyCount++;
                yield return new WaitForSeconds(enemySpawnInterval); 
            }
        }
    }
    void OnEnemyAttacked(Unit unit)
    {
        int index = enemies.IndexOf(unit);
        enemies[index].Attacked(player.GetSwordDamage);
    }
    void OnAttackPlayer(int damage)
    {
        AttackPlayer?.Invoke(damage);
    }
    void OnEnemyDestroyed(Unit unit) 
    {
        unit.Destroyed -= this.OnEnemyDestroyed;
        unit.AttackedBySword -= this.OnEnemyAttacked;
        unit.AttackPlayer -= this.OnAttackPlayer;
        int index = enemies.IndexOf(unit);
        enemies.RemoveAt(index); 
        unitFactory.Restore(unit, unit.name);
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

    public void OnGameEnded(bool isVictory, int HeartCount)
    {
        if(enemies.Count == 0)
            return;
        
        foreach(var unit in enemies)
        {
            unitFactory.Restore(unit, unit.name);
        }
    }
}
