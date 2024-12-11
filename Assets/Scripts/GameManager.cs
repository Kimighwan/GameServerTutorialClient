using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 게임 전반적인 관리 클래스

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>(); // 플레이어 컨테이너
    public static Dictionary<int, ItemSpawner> itemSpawners = new Dictionary<int, ItemSpawner>(); // 아이템 스포너 컨테이너
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>(); // 수류탄 컨테이너

    // 인스턴스화를 위한 객체들
    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject itemSpawnerPrefab;
    public GameObject projectilePrefab;
    public GameObject bulletPrefab;

    public Transform[] playerSpawner;   // 플레이어 스폰 위치
    public bool playerCheck = false;    // 플레이어가 두 명 접속했는가?
    public bool gameStart = false;      // 게임 시작 상태 변수
    public bool gameEnd = false;        // 게임 종료 상태 변수
    public bool gameResult = false;

    private void Awake()
    {
        if (instance == null) // 아직 만들어지지 않았다면 현재 객체 할당
        {
            instance = this;
        }
        else if (instance != this) // 이미 존재한다면 현재 객체 삭제
        {
            Destroy(this);
        }
    }


    public void SpawnPlayer(int id, string userName, Vector3 position, Quaternion rotation) // 플레이어 생성
    {
        GameObject player;
        if(id == Client.instance.id)
        {
            player = Instantiate(localPlayerPrefab, position, rotation);
        }
        else
        {
            player = Instantiate(playerPrefab, position, rotation);
        }

        player.GetComponent<PlayerManager>().Initialize(id, userName);
        players.Add(id, player.GetComponent<PlayerManager>());
    }

    public void CreateItemSpawner(int spawnerId, Vector3 pos, bool hasItem) // 아이템 스포너를 생성하는 함수
    {
        GameObject spawner = Instantiate(itemSpawnerPrefab, pos, itemSpawnerPrefab.transform.rotation);
        spawner.GetComponent<ItemSpawner>().Initialize(spawnerId, hasItem);
        itemSpawners.Add(spawnerId, spawner.GetComponent<ItemSpawner>());
    }

    public void SpawnProjectile(int id, Vector3 pos) // 슈류탄 생성
    {
        GameObject projectile = Instantiate(projectilePrefab, pos, Quaternion.identity);
        projectile.GetComponent<ProjectileManager>().Initialize(id);
        projectiles.Add(id, projectile.GetComponent<ProjectileManager>());
    }

    public void GameResult()    // 게임 결과 집계
    {
        int player1DieCount = players[1].dieCount;
        int player2DieCount = players[2].dieCount;

        if(player1DieCount > player2DieCount)   // 1번 플레이어가 더 많이 죽음 ==> 2번 플레이어가 승리
        {
            if(Client.instance.id == 1)
                UIManager.instance.result.text = "Lose...";
            else
                UIManager.instance.result.text = "Win!!!";
        }
        else if(player1DieCount < player2DieCount)
        {
            if (Client.instance.id == 2)
                UIManager.instance.result.text = "Lose...";
            else
                UIManager.instance.result.text = "Win!!!";
        }
        else    // 무승부
        {
            UIManager.instance.result.text = "Draw";
        }

        UIManager.instance.result.gameObject.SetActive(true);   // UI 송출
    }
}
