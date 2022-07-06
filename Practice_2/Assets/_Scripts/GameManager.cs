using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Enemy enemyPrefab;
    [SerializeField]
    int maxEnemyCount = 20;
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
        enemyManager.Initialize(new Factory(enemyPrefab), player, maxEnemyCount, enemySpawnInterval);
        fireController = new FireController(enemyManager, player);

        BindEvents();
        timeManager.StartGame();
    }
    void BindEvents()
    {
        player.FindEnemy += fireController.NearestEnemy;
        fireController.Fire += player.Fire;
        timeManager.GameStarted += player.Gamestart;
        timeManager.GameStarted += enemyManager.Gamestart;
    }

    void UnBind()
    {
        player.FindEnemy -= fireController.NearestEnemy;
        fireController.Fire -= player.Fire;
        timeManager.GameStarted -= player.Gamestart;
        timeManager.GameStarted -= enemyManager.Gamestart;
    }
}
