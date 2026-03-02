using System;
using System.Collections;
using System.Collections.Generic;
using Base.Managers;
using Base.NPC;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> 스테이지 이동정보를 저장하고 이동상황에 맞는 팝업, 초기화 실시</summary>
public class StageManager : MonoBehaviour
{
    public static StageManager Instance;
    [field: SerializeField] public int Stage { get; private set; }
    public Scenes PrevScene { get; private set; }
    public Scenes CurScene { get; private set; }

    private PassiveSkillManager rewardPopUp;
    public BattleSpawnPresetSO battlePreset;
    [SerializeField] private BonusStat enemyBonusStat = new();
    public BonusStat EnemyBonusStat => enemyBonusStat;
    [SerializeField] private BonusStat allyBonusStat = new();
    public BonusStat AllyBonusStat => allyBonusStat;

    public void Clear()
    {
        Stage = 0;
        enemyBonusStat = new();
        allyBonusStat = new();
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        PrevScene = CurScene = Scenes.WorldMap;
    }

    public void NewGame()
    {
        SetStage(0);
        //기타 초기화
    }
    public void SetStage(int stage)
    {
        Stage = stage;
    }
    /// <summary> 이전과 현재의 스테이지 종류 및 진행한 스테이지 정보를 관리</summary>
    /// <param name="enteringScene"></param>
    public void EnterStage(Scenes enteringScene)
    {
        //월드맵으로 이동할때만(직전 씬을 클리어 했을 때) 스테이지 카운트 +1
        if (enteringScene == Scenes.MainMenu) return; 
        PrevScene = CurScene;
        CurScene = enteringScene;
        if(enteringScene == Scenes.WorldMap) Stage++;
        Debug.Log($"씬 이동: {PrevScene} -> {CurScene}, 현재 스테이지 {Stage}");
    }

    
    /// <summary>현재 스테이지의 비용 성장계수</summary>
    /// <returns></returns>
    public float Growth()
    {
        double growth = 1 + 0.18 * (Stage-1) + 0.06 * (Mathf.Pow(Stage-1, 2));
        return (float)growth;
    }

}