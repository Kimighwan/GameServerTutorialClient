using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Ÿ�̸� ���� Ŭ����

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
            if (GameManager.instance.playerCheck)   // �÷��̾� 2���� �����ϸ� Ÿ�̸� ����
            {
                isDone = true;
                StartCoroutine(StartTimer());
            }
        }

        if (GameManager.instance.gameStart)      // 60�� ī��Ʈ �ٿ� ����
        {
            t -= Time.deltaTime;
            gameTimer.text = string.Format("{0:N2}", t);

            if(t <= 0f)                         // ���� ��� ���� �� ���� ���ῡ ���� UI ó��
            {
                gameTimer.gameObject.SetActive(false);
                GameManager.instance.gameStart = false;
                GameManager.instance.gameEnd = true;
                GameManager.instance.GameResult();
            }
        }
    }

    public IEnumerator StartTimer()         // 3�� �� Ÿ�̸� 60�� ī���� ����
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
