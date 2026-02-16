using System;
using System.Collections;
using System.Collections.Generic;
using Base.Manager.Test;
using Base.NPC;
using Contents.Mech;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance;
    [SerializeField] private PoolID spawner;
    private GameObject PooledNPC;
    private List<NPC> mech;
    private Transform spawnSpot;

    private void Start()
    {
        if(Instance!=null) Destroy(gameObject);
        Instance = this;
        spawnSpot = GetComponentInChildren<SpawnSpot>().gameObject.transform;
    }

    public void Spawn()
    {
        PooledNPC = PoolManager.poolDic[spawner].UsePool(spawnSpot.position, spawnSpot.rotation);
        PooledNPC.GetComponent<NPC>();
    }
}
