using System;
using System.Collections;
using System.Collections.Generic;
using Base.Manager.Test;
using Contents.Mech;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance;
    [SerializeField] private PoolID spawner;
    private MechHealth mech;
    private Transform spawnSpot;

    private void Start()
    {
        if(Instance!=null) Destroy(gameObject);
        Instance = this;
        spawnSpot = GetComponentInChildren<SpawnSpot>().gameObject.transform;
    }

    public void Spawn()
    {
        PoolManager.poolDic[spawner].UsePool(spawnSpot.position, spawnSpot.rotation);
        mech = GetComponent<MechHealth>();
    }
}
