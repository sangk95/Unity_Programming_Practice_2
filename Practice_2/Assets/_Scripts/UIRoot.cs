using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class UIRoot : MonoBehaviour
{
    [SerializeField]
    TMP_Text scoreUI; 
    [SerializeField]
    TMP_Text resultUI;
    [SerializeField]
    TMP_Text waveUI;
    [SerializeField]
    TMP_Text playerHP_UI;
    [SerializeField]
    TMP_Text playModeUI;
    bool isAutoPlayMode = false;
    public Action ChangedPlayMode;
    void Start()
    {
        scoreUI.gameObject.SetActive(false);
        resultUI.gameObject.SetActive(false);
        waveUI.gameObject.SetActive(false);
        playerHP_UI.gameObject.SetActive(false);
        playModeUI.text = string.Format("non-Auto");
    }
    public void PlayModeChange()
    {
        ChangedPlayMode?.Invoke();
        if(isAutoPlayMode)
        {
            playModeUI.text = string.Format("non-Auto");
            isAutoPlayMode = false;
        }
        else
        {
            playModeUI.text = string.Format("Auto");
            isAutoPlayMode = true;
        }
    }
    public void OnGameStarted()
    {
        scoreUI.gameObject.SetActive(true);
        scoreUI.text = string.Format("Score: {0}", 0);
        playerHP_UI.gameObject.SetActive(true);
        playerHP_UI.text = string.Format("HP : {0}",50);
    }

    public void OnGameEnded(bool isVictory, int HeartCount)
    {
        resultUI.gameObject.SetActive(true);
        resultUI.text = isVictory ? "You Win!" : "You Lose";
    }

    public void OnScoreChanged(int score)
    {
        scoreUI.text = string.Format("Score: {0}", score);
    }

    public void OnPlayerHPChanged(int hp)
    {
        playerHP_UI.text = string.Format("HP : {0}", hp);
    }

    public void OnWaveChanged(int curWave)
    {
        waveUI.gameObject.SetActive(true);
        waveUI.text = string.Format("Wave {0}", curWave);
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1.5f);
        waveUI.gameObject.SetActive(false);
    }
}
