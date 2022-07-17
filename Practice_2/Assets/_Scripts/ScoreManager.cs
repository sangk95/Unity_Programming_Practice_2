using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ScoreManager
{
    readonly int scorePerEnemy;
    readonly int scorePerHeart;
    int score;
    public Action<int> ScoreChanged;
    public ScoreManager(int scorePerEnemy = 10, int scorePerHeart = 100)
    {
        this.scorePerEnemy = scorePerEnemy;
        this.scorePerHeart = scorePerHeart;
    }

    public void OnEnemyDestroyed()
    {
        score+=scorePerEnemy;
        ScoreChanged?.Invoke(score);
    }

    public void OnGameEnded(bool isVictory, int HeartCount) 
    {
        if(HeartCount == 0)
            return;
        score += scorePerHeart * HeartCount;
        ScoreChanged?.Invoke(score);
    }
}
