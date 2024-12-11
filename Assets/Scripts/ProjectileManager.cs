using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 수류탄 관리 클래스

public class ProjectileManager : MonoBehaviour
{
    public int id;
    public GameObject explosionPrefab;  // 폭발 이펙트 객체

    public void Initialize(int _id)
    {
        id = _id;
    }

    public void Explode(Vector3 pos)    // 폭발 실행
    {
        transform.position = pos;   // 위치 고정
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);  // 폭발 이펙트 인스턴스화
        GameManager.projectiles.Remove(id); // 수류탄 컨테이너에서 삭제
        Destroy(gameObject);    // 수류탄 삭제
    }
}
