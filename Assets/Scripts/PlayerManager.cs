using UnityEngine;

// 플레이어 매니저

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string userName;
    public float hp;
    public float maxHP;
    public int itemCount = 0;   // 해당 플레이어가 가지고 있는 아이템 갯수
    public int dieCount = 0;    // 플레이어 죽은 횟수
    public MeshRenderer model;


    public void Initialize(int _id, string _userName)   // 플레이어 초기값 설정
    {
        id = _id;
        userName = _userName;
        hp = maxHP;
    }

    public void SetHP(float _hp)    // 체력 체크 함수
    {
        hp = _hp;

        if (hp <= 0f)
        {
            Die();
        }
    }

    public void Die()   // 사망
    {
        model.enabled = false;
    }

    public void ReSpawn()   // 리스폰
    {
        model.enabled = true;
        SetHP(maxHP);
    }
}