using GameServer;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

// 패킷을 읽는 함수를 정의한 클래스

// 매개변수로 패킷을 받아 해당 패킷의 데이터를 읽는다.
// 읽은 데이터를 활용하여 각 함수에 목적에 맞게 사용한다.

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string m = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log($"서버에서 받은 메세지 : {m}");
        Client.instance.id = id;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port); // tcp로 바인딩된 포트 번호로 udp 연결
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

    public static void ItemSpawned(Packet packet)   // 아이템 패킷 데이터 읽기
    {
        int spawnId = packet.ReadInt();

        GameManager.itemSpawners[spawnId].ItemSpawned();    // 아이템 ID를 이용하여 아이템 소환
    }

    public static void ItemPickedUp(Packet packet) // 아이템 획득 정보 패킷 읽기
    {
        int spawnId = packet.ReadInt();
        int byPlayer = packet.ReadInt();

        GameManager.itemSpawners[spawnId].ItemPickedUp();   // 아이템 획득했을 때 동작하는 함수 호출
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

    public static void PlayerCheck(Packet packet)
    {
        bool check = packet.ReadBool();

        GameManager.instance.playerCheck = check;
    }
}
