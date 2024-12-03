using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string userName;
    public float hp;
    public float maxHP;
    public int itemCount = 0;
    public MeshRenderer model;

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
        model.enabled = false;
    }

    public void ReSpawn()
    {
        model.enabled = true;
        SetHP(maxHP);
    }
}