using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string userName;
    public float hp;
    public float maxHP;
    public int itemCount = 0;   // 해당 플레이어가 가지고 있는 아이템 갯수
    public int dieCount = 0;    // 플레이어 죽은 횟수
    public MeshRenderer model;

    public static string result;

    private void Update()
    {
        if (GameManager.instance.gameEnd)
        {
            ClientSend.PlayerDieCount(dieCount);
            GameResult();
        }
    }

    public void Initialize(int _id, string _userName)
    {
        id = _id;
        userName = _userName;
        hp = maxHP;
    }

    public void SetHP(float _hp)
    {
        hp = _hp;

        if (hp <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        dieCount++;
        model.enabled = false;
    }

    public void ReSpawn()
    {
        model.enabled = true;
        SetHP(maxHP);
    }

    private void GameResult()
    {
        if(GameManager.instance.gameResult) // true => client 1번이 이김
        {
            if (id == 1) GameManager.instance.result.text = "WIN!!!";
            else GameManager.instance.result.text = "LOSE...";
        }
        else                               // false => client 2번이 이김
        {
            if (id == 1) GameManager.instance.result.text = "LOSE...";
            else GameManager.instance.result.text = "WIN!!!";
        }
    }
}