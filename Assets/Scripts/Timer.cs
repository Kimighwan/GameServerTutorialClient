using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 타이머 관련 클래스

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI startTimer;
    public TextMeshProUGUI gameTimer;

    private bool isDone = false;
    private float t;

    private void Awake()
    {
        t = 60f;
    }

    private void Update()
    {
        if(!isDone)
        {
            if (GameManager.instance.playerCheck)   // 플레이어 2명이 접속하면 타이머 시작
            {
                isDone = true;
                StartCoroutine(StartTimer());
            }
        }

        if (GameManager.instance.gameStart)      // 60초 카운트 다운 시작
        {
            t -= Time.deltaTime;
            gameTimer.text = string.Format("{0:N2}", t);

            if(t <= 0f)                         // 승패 결과 집계 및 게임 종료에 따른 UI 처리
            {
                gameTimer.gameObject.SetActive(false);
                GameManager.instance.gameStart = false;
                GameManager.instance.gameEnd = true;
                GameManager.instance.GameResult();
            }
        }
    }

    public IEnumerator StartTimer()         // 3초 후 타이머 60초 카운터 시작
    {
        startTimer.gameObject.SetActive(true);
        startTimer.text = "3...";
        yield return new WaitForSeconds(1f);

        startTimer.text = "2...";
        yield return new WaitForSeconds(1f);

        startTimer.text = "1...";

        yield return new WaitForSeconds(1f);

        startTimer.gameObject.SetActive(false);
        gameTimer.gameObject.SetActive(true);
        GameManager.instance.gameStart = true;
    }
}
