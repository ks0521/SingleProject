using System;
using System.Collections;
using System.Collections.Generic;
using Base.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary> 직전 방 타입에 따른 보상 제공</summary>
public class RewardPopUpController : MonoBehaviour
{
    [SerializeField] private GameObject RewardPopUp;
    [SerializeField] private PassiveSkillManager skillManager;
    private void Start()
    {
        if (ScenesManager.Instance.canPopUpReward)
        {
            GetClearSkill();
            GetClearGold();
            ScenesManager.Instance.canPopUpReward = false;
        }
    }
    /// <summary> 전투 승리 후 전투타입에 맞는 골드보상 지급</summary>
    public void GetClearGold()
    {
        int stageBonus = StageManager.Instance.Stage * 20;
        switch (StageManager.Instance.PrevScene)
        {
            case Scenes.Battle:
                PlayerInfoManager.Instance.GetGold(Random.Range(150 + stageBonus,170 + stageBonus));
                break;
            case Scenes.Elite:
                PlayerInfoManager.Instance.GetGold(Random.Range(250 + stageBonus,300 + stageBonus));
                break;
            case Scenes.Boss: 
                PlayerInfoManager.Instance.GetGold(Random.Range(400 + stageBonus,500 + stageBonus));
                break;
        }
    }

    public void GetClearSkill()
    {
        RewardPopUp.SetActive(true);
        skillManager.MakeReward(StageManager.Instance.PrevScene);
    }
}
