using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>(); // �÷��̾� �����̳�
    public static Dictionary<int, ItemSpawner> itemSpawners = new Dictionary<int, ItemSpawner>(); // ������ ������ �����̳�
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject itemSpawnerPrefab;
    public GameObject projectilePrefab;

    private void Awake()
    {
        if (instance == null) // ���� ��������� �ʾҴٸ� ���� ��ü �Ҵ�
        {
            instance = this;
        }
        else if (instance != this) // �̹� �����Ѵٸ� ���� ��ü ����
        {
            Destroy(this);
        }
    }

    public void SpawnPlayer(int id, string userName, Vector3 position, Quaternion rotation)
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

    public void CreateItemSpawner(int spawnerId, Vector3 pos, bool hasItem) // ������ �����ʸ� �����ϴ� �Լ�
    {
        GameObject spawner = Instantiate(itemSpawnerPrefab, pos, itemSpawnerPrefab.transform.rotation);
        spawner.GetComponent<ItemSpawner>().Initialize(spawnerId, hasItem);
        itemSpawners.Add(spawnerId, spawner.GetComponent<ItemSpawner>());
    }

    public void SpawnProjectile(int id, Vector3 pos)
    {
        GameObject projectile = Instantiate(projectilePrefab, pos, Quaternion.identity);
        projectile.GetComponent<ProjectileManager>().Initialize(id);
        projectiles.Add(id, projectile.GetComponent<ProjectileManager>());
    }
}
