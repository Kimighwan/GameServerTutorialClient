using GameServer;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

// ��Ŷ�� �д� �Լ��� ������ Ŭ����

// �Ű������� ��Ŷ�� �޾� �ش� ��Ŷ�� �����͸� �д´�.
// ���� �����͸� Ȱ���Ͽ� �� �Լ��� ������ �°� ����Ѵ�.

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string m = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log($"�������� ���� �޼��� : {m}");
        Client.instance.id = id;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port); // tcp�� ���ε��� ��Ʈ ��ȣ�� udp ����
    }

    public static void SpawnPlayer(Packet packet)
    {
        int _id = packet.ReadInt();
        string _userName = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _userName, position, rotation);
    }

    public static void PlayerPosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        GameManager.players[id].transform.position = position;
    }

    public static void PlayerRotation(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.players[id].transform.rotation = rotation;
    }

    public static void PlayerDisconnected(Packet packet)
    {
        int id = packet.ReadInt();

        Destroy(GameManager.players[id].gameObject);
        GameManager.players.Remove(id);
    }

    public static void PlayerHP(Packet packet)
    {
        int id = packet.ReadInt();
        float hp = packet.ReadFloat();

        GameManager.players[id].SetHP(hp);
    }

    public static void PlayerReSpawned(Packet packet)
    {
        int id = packet.ReadInt();

        GameManager.players[id].ReSpawn();
    }

    public static void CreateItemSpawner(Packet packet)
    {
        int spawnerId = packet.ReadInt();
        Vector3 spawnerPos = packet.ReadVector3();
        bool hasItem = packet.ReadBool();

        GameManager.instance.CreateItemSpawner(spawnerId, spawnerPos, hasItem);
    }

    public static void ItemSpawned(Packet packet)   // ������ ��Ŷ ������ �б�
    {
        int spawnId = packet.ReadInt();

        GameManager.itemSpawners[spawnId].ItemSpawned();    // ������ ID�� �̿��Ͽ� ������ ��ȯ
    }

    public static void ItemPickedUp(Packet packet) // ������ ȹ�� ���� ��Ŷ �б�
    {
        int spawnId = packet.ReadInt();
        int byPlayer = packet.ReadInt();

        GameManager.itemSpawners[spawnId].ItemPickedUp();   // ������ ȹ������ �� �����ϴ� �Լ� ȣ��
        GameManager.players[byPlayer].itemCount++;
    }

    public static void SpawnProjectile(Packet packet)
    {
        int projectileId = packet.ReadInt();
        Vector3 pos = packet.ReadVector3();
        int throwByPlayer = packet.ReadInt();

        GameManager.instance.SpawnProjectile(projectileId, pos);
        GameManager.players[throwByPlayer].itemCount--;
    }

    public static void ProjectilePosition(Packet packet)
    {
        int projectileId = packet.ReadInt();
        Vector3 pos = packet.ReadVector3();

        GameManager.projectiles[projectileId].transform.position = pos;
    }

    public static void ProjectileExploded(Packet packet)
    {
        int projectileId = packet.ReadInt();
        Vector3 pos = packet.ReadVector3();

        GameManager.projectiles[projectileId].Explode(pos);
    }
}
