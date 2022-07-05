using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TimeManager : MonoBehaviour
{
    bool isGameStarted = false;
    public Action GameStarted;
    public void StartGame(float time = 3f)
    {
        if(isGameStarted)
            return;
        isGameStarted = true;
        StartCoroutine(DelayGameStart(time));
    }

    IEnumerator DelayGameStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameStarted?.Invoke();
    }
}
