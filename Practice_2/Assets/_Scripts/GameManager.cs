using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    [SerializeField]
    int scorePerEnemy = 10;
    [SerializeField]
    int scorePerHP = 100;
    [SerializeField]
    Unit[] unitPrefab;
    [SerializeField]
    int maxWave = 2;
    [SerializeField]
    int waveEnemyCount = 5;
    [SerializeField]
    float waveInterval = 5f;
    [SerializeField]
    float enemySpawnInterval = 0.5f;
    [SerializeField]
    Transform playerPosition;
    [SerializeField]
    PlayerController playerPrefab;
    [SerializeField]
    UIRoot uIRoot;
    [SerializeField]
    BackGround[] backGround;
    
    public Action<bool, int> GameEnded;
    bool isAllHPDestroyed = false;

    PlayerController player;
    FireController fireController;
    EnemyManager enemyManager;
    TimeManager timeManager;
    ScoreManager scoreManager;
    void Start()
    {
        player = Instantiate(playerPrefab);
        player.transform.position = playerPosition.position;
        timeManager = gameObject.AddComponent<TimeManager>();
        enemyManager = gameObject.AddComponent<EnemyManager>();
        enemyManager.Initialize(new UnitFactory(unitPrefab), player, maxWave, waveEnemyCount, waveInterval, enemySpawnInterval);
        fireController = new FireController(enemyManager, player);
        scoreManager = new ScoreManager(scorePerEnemy, scorePerHP);

        BindEvents();
        timeManager.StartGame();
    }
    void BindEvents()
    {
        player.AllHPDestroyed += this.OnAllHPDestroyed;
        player.StopMoved += enemyManager.resetActivate;
        player.PlayerAttacked += uIRoot.OnPlayerHPChanged;
        player.SkillUsed += uIRoot.SkillCoolCount;

        //fireController.Fire += player.FireReady;
        enemyManager.NextStage += fireController.NearestEnemy;
        enemyManager.EnemyDestroyed += scoreManager.OnEnemyDestroyed;
        enemyManager.AllEnemyDestroyed += this.OnAllEnemyDestroyed;
        enemyManager.WaveStarted += uIRoot.OnWaveChanged;
        enemyManager.WaveEnd += player.SetDefaultPosition;

        enemyManager.AttackPlayer += player.Attacked;
        foreach(var back in backGround)
        {
            enemyManager.MovingToNextWave += back.checkMove;
        }
        timeManager.GameStarted += player.Gamestart;
        timeManager.GameStarted += enemyManager.Gamestart;
        timeManager.GameStarted += uIRoot.OnGameStarted;
        scoreManager.ScoreChanged += uIRoot.OnScoreChanged;
        uIRoot.ChangedPlayMode += player.PlayModeChange;

        this.GameEnded += player.OnGameEnded;
        this.GameEnded += enemyManager.OnGameEnded;
        this.GameEnded += scoreManager.OnGameEnded;
        this.GameEnded += uIRoot.OnGameEnded;
    }

    void UnBindEvents()
    {
        player.AllHPDestroyed -= this.OnAllHPDestroyed;
        player.StopMoved -= enemyManager.resetActivate;
        player.PlayerAttacked -= uIRoot.OnPlayerHPChanged;
        player.SkillUsed -= uIRoot.SkillCoolCount;

        //fireController.Fire -= player.FireReady;
        enemyManager.NextStage -= fireController.NearestEnemy;
        enemyManager.EnemyDestroyed -= scoreManager.OnEnemyDestroyed;
        enemyManager.AllEnemyDestroyed -= this.OnAllEnemyDestroyed;
        enemyManager.WaveStarted -= uIRoot.OnWaveChanged;
        enemyManager.WaveEnd -= player.SetDefaultPosition;
        enemyManager.AttackPlayer -= player.Attacked;
        foreach(var back in backGround)
        {
            enemyManager.MovingToNextWave -= back.checkMove;
        }
        timeManager.GameStarted -= player.Gamestart;
        timeManager.GameStarted -= enemyManager.Gamestart;
        timeManager.GameStarted -= uIRoot.OnGameStarted;
        scoreManager.ScoreChanged -= uIRoot.OnScoreChanged;
        uIRoot.ChangedPlayMode -= player.PlayModeChange;
        
        this.GameEnded -= player.OnGameEnded;
        this.GameEnded -= enemyManager.OnGameEnded;
        this.GameEnded -= scoreManager.OnGameEnded;
        this.GameEnded -= uIRoot.OnGameEnded;
    }
    
    void OnDestroy()
    {
        UnBindEvents();   
    }

    void OnAllHPDestroyed()
    {
        isAllHPDestroyed = true;
        GameEnded?.Invoke(false, player.HPCount);
        AudioManager.instance.PlaySound(SoundId.GameEnd); 
    }
    void OnAllEnemyDestroyed()
    {
        StartCoroutine(DelayedGameEnded()); 
    }
    IEnumerator DelayedGameEnded()
    {
        yield return null;
        if(!isAllHPDestroyed)
        {
            GameEnded?.Invoke(true, player.HPCount);
            AudioManager.instance.PlaySound(SoundId.GameEnd); 
        }
    }

}
