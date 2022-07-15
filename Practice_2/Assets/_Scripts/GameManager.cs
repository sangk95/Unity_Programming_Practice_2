using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Transform[] backGround;
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
    PlayerController player;
    FireController fireController;
    EnemyManager enemyManager;
    TimeManager timeManager;
    // Start is called before the first frame update
    void Start()
    {
        player = Instantiate(playerPrefab);
        player.transform.position = playerPosition.position;
        timeManager = gameObject.AddComponent<TimeManager>();
        enemyManager = gameObject.AddComponent<EnemyManager>();
        enemyManager.Initialize(new UnitGenerator(unitPrefab), player, maxWave, waveEnemyCount, waveInterval, enemySpawnInterval);
        fireController = new FireController(enemyManager, player);

        BindEvents();
        timeManager.StartGame();
    }
    void BindEvents()
    {
        player.FindEnemy += fireController.NearestEnemy;
        player.HitEnemy += enemyManager.Attacked;
        fireController.Fire += player.FireReady;
        fireController.Fire += enemyManager.resetActivate;
        enemyManager.NextStage += fireController.NearestEnemy;
        timeManager.GameStarted += player.Gamestart;
        timeManager.GameStarted += enemyManager.Gamestart;
    }

    void UnBind()
    {
        player.FindEnemy -= fireController.NearestEnemy;
        player.HitEnemy -= enemyManager.Attacked;
        fireController.Fire -= player.FireReady;
        fireController.Fire -= enemyManager.resetActivate;
        enemyManager.NextStage -= fireController.NearestEnemy;
        timeManager.GameStarted -= player.Gamestart;
        timeManager.GameStarted -= enemyManager.Gamestart;
    }
}
