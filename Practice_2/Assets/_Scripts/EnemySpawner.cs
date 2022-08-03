using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemySpawnDatabase
{
    public string Stage, Wave, EnemyType, Count;
    public EnemySpawnDatabase(string Stage, string Wave, string EnemyType, string Count)
    {
        this.Stage = Stage;
        this.Wave = Wave;
        this.EnemyType = EnemyType;
        this.Count = Count;
    }
}
public class EnemyStatDatabase
{
    public string Name, Speed, HP, ATK;
    public EnemyStatDatabase(string Name, string Speed, string HP, string ATK)
    {
        this.Name = Name;
        this.Speed = Speed;
        this.HP = HP;
        this.ATK = ATK;
    }
}
public class EnemySpawner
{
    TextAsset enemySpawnDatabase;
    TextAsset enemyStatDatabase;
    [SerializeField]
    List<EnemySpawnDatabase> spawnDB = new List<EnemySpawnDatabase>(); 
    List<EnemyStatDatabase> statDB = new List<EnemyStatDatabase>();
    int curStage;
    int maxWave;
    Dictionary<string, int> EnemyTypeCount = new Dictionary<string, int>();

    public List<EnemyStatDatabase> GetEnemyStatData => statDB;
    public int GetMaxWave => int.Parse(spawnDB[spawnDB.Count-1].Wave);
    public Dictionary<string, int> GetEnemyTypeCount(int curWave)
    {
        EnemyTypeCount.Clear();
        foreach(var data in spawnDB)
            if(data.Wave == curWave.ToString())
                EnemyTypeCount.Add(data.EnemyType, int.Parse(data.Count));

        return EnemyTypeCount;
    }
    public EnemySpawner(int curStage, TextAsset enemySpawnDatabase, TextAsset enemyStatDatabase)
    {
        this.curStage = curStage;
        this.enemySpawnDatabase = enemySpawnDatabase;
        this.enemyStatDatabase = enemyStatDatabase;

        Debug.Assert(this.enemySpawnDatabase != null, "EnemySpawnDatabase is null!");
        Debug.Assert(this.enemyStatDatabase != null, "EnemyStatDatabase is null!");
        
        Setting();
    }
    void Setting()
    {
        string[] spawn = enemySpawnDatabase.text.Substring(0, enemySpawnDatabase.text.Length-1).Split('\n');
        for(int i=1 ; i<spawn.Length ; i++)
        {
            string[] row = spawn[i].Split('\t');
            if(int.Parse(row[0]) == curStage)
                spawnDB.Add(new EnemySpawnDatabase(row[0], row[1], row[2], row[3]));
        }

        string[] stat = enemyStatDatabase.text.Substring(0, enemyStatDatabase.text.Length-1).Split('\n');
        for(int i=1 ; i<stat.Length ; i++)
        {
            string[] row = stat[i].Split('\t');
            statDB.Add(new EnemyStatDatabase(row[0], row[1], row[2], row[3]));
        }
    }
}
