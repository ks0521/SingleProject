using System;
using System.Collections;
using System.Collections.Generic;
using Base.NPC;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;
    [SerializeField] private List<NPC> enemyList;
    [SerializeField] private List<NPC> allyList;
    [field:SerializeField]
    public int Stage { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    public void SetStage(int stage)
    {
        Stage = stage;
    }

    public void SpawnAlly()
    {
        
    }

    public void SpwanEnemy()
    {
        
    }
    public void StageProgress() => Stage++;
}
