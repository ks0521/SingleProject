using System;
using System.Collections;
using System.Collections.Generic;
using Base.Manager.Test;
using Base.NPC;
using Contnts.Player;
using SO.Mech;
using UnityEngine;
using Random = System.Random;

public enum NpcType
{
    Ally,
    Enemy,
    Elite,
    Boss
}

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance;
    [SerializeField] private List<MechArcheTypeSO> _NpcArcheType;
    [SerializeField] private PoolID Player;
    [SerializeField] private PoolID Enemy;
    [SerializeField] private PoolID Ally;
    [SerializeField] private PoolID Elite;
    [SerializeField] private PoolID Boss;
    [SerializeField] private SpawnGroup playerSpawnPoints;
    [SerializeField] private SpawnGroup allySpawnPoints;
    [SerializeField] private SpawnGroup enemySpawnPoints;
    [SerializeField] private SpawnGroup reinforceSpawnPoints;
    [SerializeField] private HUDManager hudManager;
    private GameObject PooledNPC;
    private List<GameObject> _allyList = new();
    private List<GameObject> _enemyList = new();
    public IReadOnlyList<GameObject> AllyList => _allyList;
    public IReadOnlyList<GameObject> EnemyList => _enemyList;
    public PlayerInfoManager playerInfo => PlayerInfoManager.Instance;
    public event Action<int> OnAllyNpcRemain;
    public event Action<int> OnEnemyNpcRemain;
    public event Action OnAllEnemiesBroken;
    public event Action<int, int> OnSpawnFinished; //아군 및 적 생성완료

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _allyList = new List<GameObject>();
        _enemyList = new List<GameObject>();

        Instance = this;
    }

    private void OnEnable()
    {
        StartSpawn(StageManager.Instance.battlePreset);
    }

    public void StartSpawn(in BattleSpawnPresetSO preset)
    {
        PlayerSpawn();
        // 적 스폰
        SpawnRangeGroup(enemySpawnPoints, isEnemy: true, preset.enemy);
        // 아군 스폰 (원하면)
        SpawnRangeGroup(allySpawnPoints, isEnemy: false, preset.ally);
        // 엘리트 (옵션)
        Debug.Log($"{_allyList.Count},{_enemyList.Count}");
        OnSpawnFinished?.Invoke(_allyList.Count, _enemyList.Count);
    }

    private void SpawnRangeGroup(SpawnGroup group, bool isEnemy, RangeCount count)
    {
        for (int i = 0; i < count.shortRange; i++)
            Spawn(group, isEnemy, MechRangeType.Short);
        for (int i = 0; i < count.midRange; i++)
            Spawn(group, isEnemy, MechRangeType.Middle);
        for (int i = 0; i < count.longRange; i++)
            Spawn(group, isEnemy, MechRangeType.Long);
    }


    public void Spawn(SpawnGroup group, bool isEnemy, MechRangeType range)
    {
        var spawnPoint = group.UseSpot();
        if (spawnPoint == null)
        {
            Debug.LogWarning("스폰지점 없음");
            return;
        }
        int archetype = 0;
        switch (range) 
        {
            case MechRangeType.Short:
                archetype = (int)MechRangeType.Short;
                break;
            case MechRangeType.Middle:
                archetype = (int)MechRangeType.Middle;
                break;
            case MechRangeType.Long:
                archetype = (int)MechRangeType.Long;
                break;
        }
        if (!isEnemy) //아군 생성
        {
            PooledNPC = PoolManager.poolDic[Ally]
                .UsePool(spawnPoint.transform.position, spawnPoint.transform.rotation, false);
            _allyList.Add(PooledNPC);
            PooledNPC.GetComponent<MechStatus>().Init(StageManager.Instance.AllyBonusStat);
            OnAllyNpcRemain?.Invoke(_allyList.Count);
        }
        else //적군 생성
        {
            PooledNPC = PoolManager.poolDic[Enemy]
                .UsePool(spawnPoint.transform.position, spawnPoint.transform.rotation, false);
            _enemyList.Add(PooledNPC);
            PooledNPC.GetComponent<MechStatus>().Init(StageManager.Instance.EnemyBonusStat);
            OnEnemyNpcRemain?.Invoke(_enemyList.Count);
        }
        PooledNPC.GetComponent<MechStatus>().archeType = _NpcArcheType[archetype];
        PooledNPC.GetComponent<MechWeaponInventory>().Init(_NpcArcheType[archetype].weaponLoadOut.weapons);
        PooledNPC.SetActive(true);
        Debug.Log($"{PooledNPC}타입 {(MechRangeType)archetype} 생성");
        DieHooking(PooledNPC);
    }

    public void PlayerSpawn()
    {
        var spawnPoint = playerSpawnPoints.UseSpot();
        GameObject pooledPlayer = PoolManager.poolDic[Player]
            .UsePool(spawnPoint.transform.position, spawnPoint.transform.rotation, false);
        pooledPlayer.GetComponent<MechStatus>().Init(playerInfo.GetStatus());
        pooledPlayer.GetComponent<MechHealth>().Init(playerInfo.PlayerHp, playerInfo.PlayerMaxHp);
        pooledPlayer.GetComponent<MechWeaponInventory>().Init(playerInfo.GetPlayerWeaponSettings());
        pooledPlayer.GetComponent<PlayerAim>().Init();
        pooledPlayer.SetActive(true);
        _allyList.Add(pooledPlayer);
        hudManager.PlayerActivated();
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