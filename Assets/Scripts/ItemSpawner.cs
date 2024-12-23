﻿using UnityEngine;

// 아이템 스포너 클래스

public class ItemSpawner : MonoBehaviour
{
    public int spawnerId;
    public bool hasItem;
    public MeshRenderer itemModel;

    public float itemRotationSpeed = 50f;
    public float itemBobSpeed = 2f;
    private Vector3 basePos;

    public GameObject particle;

    private void Awake()    // 초기 위치 설정
    {
        particle.transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        if(hasItem)
        {
            transform.Rotate(Vector3.up, itemRotationSpeed * Time.deltaTime, Space.World);
            // Vector3.up을 기준으로 회전하는 함수
            // Vector3.up는 (0, 1, 0)을 의미

            transform.position = basePos + new Vector3(0f, 0.25f * Mathf.Sin(Time.time * itemBobSpeed), 0f);
            // y축은 sin 그래프의 y축 변화량을 가짐
            // 시간에 따라 오브젝트가 sin함수 변화량에 따라 위아래로 움직인다
        }
    }

    public void Initialize(int _spawnerId, bool _hasItem)   // 스포너 설정값 할당
    {
        spawnerId = _spawnerId;
        hasItem = _hasItem;
        itemModel.enabled = _hasItem;

        basePos = transform.position;
    }

    public void ItemSpawned()   // 아이템 스폰
    {
        hasItem = true;
        itemModel.enabled = true;
    }

    public void ItemPickedUp()  // 아이템 누군가에 의해 획득됨
    {
        hasItem = false;
        itemModel.enabled = false;
    }
}
