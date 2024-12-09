using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI startTimer;
    public TextMeshProUGUI gameTimer;

    private bool isDone = false;

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
        GameManager.instance.gameStart = true;
    }

    private void Time()
    {

    }
}
