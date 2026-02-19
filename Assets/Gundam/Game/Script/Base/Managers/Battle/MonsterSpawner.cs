using System;
using System.Collections;
using System.Collections.Generic;
using Base.Manager.Test;
using Base.NPC;
using Contents.Mech;
using UnityEngine;

public enum NpcType
{
    Ally,
    Enemy,
    Elite,
    Boss
}

public struct StageSpawnData
{
    public int allyCount;
    public int enemyCount;
    public int eliteCount;
    public int bossCount;

    public int curStage;

    public StageSpawnData(int ally, int enemy, int elite, int boss, int stage)
    {
        allyCount = ally;
        enemyCount = enemy;
        eliteCount = elite;
        bossCount = boss;
        curStage = stage;
    }
}

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance;

    [SerializeField] private PoolID Enemy;
    [SerializeField] private PoolID Ally;
    [SerializeField] private PoolID Elite;
    [SerializeField] private PoolID Boss;
    [SerializeField] private SpawnGroup AllySpawnGroup;
    [SerializeField] private SpawnGroup EnemySpawnGroup;
    [SerializeField] private SpawnGroup ReinforceSpawnGroup;
    private GameObject PooledNPC;
    private List<GameObject> _allyList;
    private List<GameObject> _enemyList;
    public event Action<int> OnAllyNpcRemain;
    public event Action<int> OnEnemyNpcRemain;
    public event Action OnAllEnemiesBroken;

    public event Action<int, int> OnSpawnFinished; //아군 및 적 생성완료

    private void Awake()
    {
        _allyList = new List<GameObject>();
        _enemyList = new List<GameObject>();
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StartSpawn(in StageSpawnData data)
    {
        for (int i = 0; i < data.allyCount; i++) Spawn(AllySpawnGroup, NpcType.Ally);
        for (int i = 0; i < data.enemyCount; i++) Spawn(EnemySpawnGroup, NpcType.Enemy);
        for (int i = 0; i < data.eliteCount; i++) Spawn(EnemySpawnGroup, NpcType.Elite);
        for (int i = 0; i < data.bossCount; i++) Spawn(EnemySpawnGroup, NpcType.Boss);

        Debug.Log($"{_allyList.Count},{_enemyList.Count}");
        OnSpawnFinished?.Invoke(_allyList.Count, _enemyList.Count);
    }

    public void Spawn(SpawnGroup group, NpcType type)
    {
        var spawnPoint = group.UseSpot();
        if (spawnPoint == null)
        {
            Debug.LogWarning("스폰지점 없음");
            return;
        }

        if (type == NpcType.Ally)
        {
            PooledNPC = PoolManager.poolDic[Ally].UsePool(spawnPoint.transform.position, spawnPoint.transform.rotation);
            _allyList.Add(PooledNPC);
            OnAllyNpcRemain?.Invoke(_allyList.Count);
            DieHooking(PooledNPC);
        }
        else
        {
            switch (type)
            {
                case NpcType.Enemy:
                    PooledNPC = PoolManager.poolDic[Enemy]
                                           .UsePool(spawnPoint.transform.position, spawnPoint.transform.rotation);
                    break;
                case NpcType.Elite:
                    PooledNPC = PoolManager.poolDic[Elite]
                                           .UsePool(spawnPoint.transform.position, spawnPoint.transform.rotation);
                    break;
                case NpcType.Boss:
                    PooledNPC = PoolManager.poolDic[Boss]
                                           .UsePool(spawnPoint.transform.position, spawnPoint.transform.rotation);
                    break;
                default:
                    break;
            }

            _enemyList.Add(PooledNPC);
            OnEnemyNpcRemain?.Invoke(_enemyList.Count);
            DieHooking(PooledNPC);
        }
    }

    void DieHooking(GameObject mech)
    {
        if (mech.TryGetComponent(out MechHealth mechHealth))
        {
            mechHealth.OnDie -= MechDie; //기존 중복제거
            mechHealth.OnDie += MechDie;
        }
    }

    void MechDie(MechHealth deadMech)
    {
        deadMech.OnDie -= MechDie;
        if (_allyList.Contains(deadMech.gameObject))
        {
            Debug.Log($"Ally {deadMech.gameObject} 삭제");
            _allyList.Remove(deadMech.gameObject);
            OnAllyNpcRemain?.Invoke(_allyList.Count);
        }
        else
        {
            Debug.Log($"Enemy {deadMech.gameObject} 삭제");
            _enemyList.Remove(deadMech.gameObject);
            OnEnemyNpcRemain?.Invoke(_enemyList.Count);
            if (_enemyList.Count <= 0)
            {
                Debug.Log("스테이지 클리어");
                Time.timeScale = 0;
                OnAllEnemiesBroken?.Invoke();
            }
        }
    }
}