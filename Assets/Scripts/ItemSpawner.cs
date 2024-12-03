using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public int spawnerId;
    public bool hasItem;
    public MeshRenderer itemModel;

    public float itemRotationSpeed = 50f;
    public float itemBobSpeed = 2f;
    private Vector3 basePos;

    private void Update()
    {
        if(hasItem)
        {
            transform.Rotate(Vector3.up, itemRotationSpeed * Time.deltaTime, Space.World);
            transform.position = basePos + new Vector3(0f, 0.25f * Mathf.Sin(Time.time * itemBobSpeed), 0f);
            // y축은 sin 그래프의 y축 변화량을 가짐
        }
    }

    public void Initialize(int _spawnerId, bool _hasItem)
    {
        spawnerId = _spawnerId;
        hasItem = _hasItem;
        itemModel.enabled = _hasItem;

        basePos = transform.position;
    }

    public void ItemSpawned()
    {
        hasItem = true;
        itemModel.enabled = true;
    }

    public void ItemPickedUp()
    {
        hasItem = false;
        itemModel.enabled = false;
    }
}
