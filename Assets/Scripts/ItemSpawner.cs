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
            // Vector3.up�� �������� ȸ���ϴ� �Լ�
            // Vector3.up�� (0, 1, 0)�� �ǹ�

            transform.position = basePos + new Vector3(0f, 0.25f * Mathf.Sin(Time.time * itemBobSpeed), 0f);
            // y���� sin �׷����� y�� ��ȭ���� ����
            // �ð��� ���� ������Ʈ�� sin�Լ� ��ȭ���� ���� ���Ʒ��� �����δ�
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
