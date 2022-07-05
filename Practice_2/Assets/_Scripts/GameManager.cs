using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    PlayerController playerPrefab;
    PlayerController player;
    MouseController mouseController;
    TimeManager timeManager;
    // Start is called before the first frame update
    void Start()
    {
        player = Instantiate(playerPrefab);
        mouseController = gameObject.AddComponent<MouseController>();
        timeManager = gameObject.AddComponent<TimeManager>();

        BindEvents();
        timeManager.StartGame();
    }
    void BindEvents()
    {
        mouseController.FireButtonPressed += player.Fire;
        timeManager.GameStarted += player.Gamestart;
    }

    void UnBind()
    {
        mouseController.FireButtonPressed -= player.Fire;
        timeManager.GameStarted -= player.Gamestart;
    }
}
