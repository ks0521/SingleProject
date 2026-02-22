using System;
using System.Collections;
using System.Collections.Generic;
using Base.Managers;
using Base.NPC;
using UnityEngine;
/// <summary> 스테이지 이동정보를 저장하고 이동상황에 맞는 팝업, 초기화 실시</summary>
public class StageManager : MonoBehaviour
{
    public static StageManager Instance;
    [field: SerializeField] public int Stage { get; private set; }
    public Scenes PrevScene { get; private set; }
    public Scenes CurScene { get; private set; }

    private PassiveSkillManager rewardPopUp;
    [SerializeField] private List<NPC> enemyList;
    [SerializeField] private List<NPC> allyList;
    

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

    public void SetStage(int stage)
    {
        Stage = stage;
    }
    /// <summary> 이전과 현재의 스테이지 종류 및 진행한 스테이지 정보를 관리</summary>
    /// <param name="enteringScene"></param>
    public void EnterStage(Scenes enteringScene)
    {
        if (enteringScene == Scenes.MainMenu) return; //메인메뉴로 가는건 스테이지 이동 아님
        PrevScene = CurScene;
        CurScene = enteringScene;
        StageProgress();
        Debug.Log($"씬 이동: {PrevScene} -> {CurScene}, 현재 스테이지 {Stage}");
    }

    public void StageProgress() => Stage++;

    public void SpawnAlly()
    {
    }

    public void SpwanEnemy()
    {
    }
}