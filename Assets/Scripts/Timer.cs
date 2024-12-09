using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
            if (GameManager.instance.playerCheck)
            {
                isDone = true;
                StartCoroutine(StartTimer());
            }
        }

        if (GameManager.instance.gameStart)
        {
            t -= Time.deltaTime;
            gameTimer.text = string.Format("{0:N2}", t);

            if(t <= 0f)
            {
                gameTimer.gameObject.SetActive(false);
                GameManager.instance.gameStart = false;
            }
        }
    }

    public IEnumerator StartTimer()
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
