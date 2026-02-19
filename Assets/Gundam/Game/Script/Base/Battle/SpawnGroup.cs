using System;
using System.Collections;
using System.Collections.Generic;
using Base.Battle;
using UnityEngine;

public class SpawnGroup : MonoBehaviour
{
    [SerializeField] private SpawnPoint[] spots;
    private SpawnPoint spawnPoint;
    private void Awake()
    {
        spots = GetComponentsInChildren<SpawnPoint>(true);
    }

    public SpawnPoint UseSpot()
    {
        spawnPoint = Array.Find(spots, point => point.TryUse());
        return spawnPoint;
    }
}
