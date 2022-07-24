using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIRoot : MonoBehaviour
{
    [SerializeField]
    TMP_Text scoreUI; 
    [SerializeField]
    TMP_Text resultUI;
    [SerializeField]
    TMP_Text waveUI;
    void Start()
    {
        scoreUI.gameObject.SetActive(false);
        resultUI.gameObject.SetActive(false);
        waveUI.gameObject.SetActive(false);
    }

    public void OnGameStarted()
    {
        scoreUI.gameObject.SetActive(true);
        scoreUI.text = string.Format("Score: {0}", 0);
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
