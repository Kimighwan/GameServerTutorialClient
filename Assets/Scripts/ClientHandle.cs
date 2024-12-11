using GameServer;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

// 패킷에서 Byte인 데이터들을 적절한 데이터형으로 읽습니다.
// 읽은 데이터를 가지고 각 함수에서 하고자 하는 일을 처리합니다.

public class ClientHandle : MonoBehaviour
{
    public static void Init(Packet packet) // 초기값 설정 패킷 수신
    {
        string m = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log($"서버에서 받은 메세지 : {m}");
        Client.instance.id = id;
        ClientSend.Init();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port); // tcp로 바인딩된 포트 번호로 udp 연결
    }

    public static void SpawnPlayer(Packet packet)  // 플레이어 생성 정보 수신
    {
        int _id = packet.ReadInt();
        string _userName = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _userName, position, rotation);
    }

    public static void PlayerPosition(Packet packet)    // 플레이어 움직임 정보 수신
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        GameManager.players[id].transform.position = position;
    }

    public static void PlayerRotation(Packet packet)    // 플레이어 회전 정보 수신
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.players[id].transform.rotation = rotation;
    }

    public static void PlayerDisconnected(Packet packet)    // 플레이어 연결 해제 정보 수신
    {
        int id = packet.ReadInt();

        Destroy(GameManager.players[id].gameObject);
        GameManager.players.Remove(id);
    }

    public static void PlayerHP(Packet packet)  // 플레이아 체력 정보 수신
    {
        int id = packet.ReadInt();
        float hp = packet.ReadFloat();

        GameManager.players[id].SetHP(hp);
    }

    public static void PlayerReSpawned(Packet packet)   // 플레이어 리스폰 정보 수신
    {
        int id = packet.ReadInt();

        GameManager.players[id].ReSpawn();
    }

    public static void CreateItemSpawner(Packet packet) // 아이템 생성기 정보 수신
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

    public static void SpawnProjectile(Packet packet)   // 수류탄 생성 정보 수신
    {
        int projectileId = packet.ReadInt();
        Vector3 pos = packet.ReadVector3();
        int throwByPlayer = packet.ReadInt();

        GameManager.instance.SpawnProjectile(projectileId, pos);
        GameManager.players[throwByPlayer].itemCount--;
    }

    public static void ProjectilePosition(Packet packet)     // 수류탄 위치 정보 수신
    {
        int projectileId = packet.ReadInt();
        Vector3 pos = packet.ReadVector3();

        GameManager.projectiles[projectileId].transform.position = pos; // 수류탄 위치 계속 동기화
    }

    public static void ProjectileExploded(Packet packet)    // 수류탄 폭발 정보 수신
    {
        int projectileId = packet.ReadInt();
        Vector3 pos = packet.ReadVector3();

        if(GameManager.projectiles.ContainsKey(projectileId))
            GameManager.projectiles[projectileId].Explode(pos);
    }

    public static void PlayerCheck(Packet packet)   // 플레이어 수 정보 수신
    {
        bool check = packet.ReadBool();

        GameManager.instance.playerCheck = check;
    }

    public static void PlayerDieCount(Packet packet)    // 플레이어 죽은 횟수 정보 수신
    {
        int id = packet.ReadInt();

        GameManager.players[id].dieCount++;
    }
}
