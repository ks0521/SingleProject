using System;
using System.Collections;
using System.Collections.Generic;
using SO.Weapon;
using SO.Mech;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary> 플레이어의 보유 패시브, 골드, 체력, 손상도 관리
/// 전투 시작시 여기서 Hp와 손상도를 받아와 초기 세팅
/// 전투 종료시 Hp와 손상도를 여기(PlayerInfoManager)에 갱신</summary>
public class PlayerInfoManager : MonoBehaviour
{
    public static PlayerInfoManager Instance;
    private MechRangeType rangeType;
    public MechRangeType RangeType => rangeType;
    private List<PassiveSkillSO> _gainedSkill;
    private List<WeaponData> weapons;
    private int playerHp;
    private int playerBreakdown; // 손상도
    public int Gold { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _gainedSkill = new List<PassiveSkillSO>();
    }

    public List<WeaponData> GetPlayerWeaponSettings() { return weapons; }

public void SelectType(MechRangeType rangeType)
    {
        this.rangeType = rangeType;
        Debug.Log($"플레이어 기체 타입 선택 : {this.rangeType}");
    }
    /// <summary> 새로 획득한 스킬을 보유중인 스킬 리스트에 추가</summary>
    /// <param name="newSkill">획득한 스킬</param>
    public void GetSkill(PassiveSkillSO newSkill)
    {
        _gainedSkill.Add(newSkill);
        Debug.Log("새로운 스킬 추가. 현재 획득한 스킬");
        foreach (var skill in _gainedSkill)
        {
            Debug.Log(skill);
        }
    }

    
    /// <summary> 상시 발동형인 패시브의 스탯 증가량을 다 더해서 전달</summary>
    /// <returns> 패시브 스킬이 적용된 스킬들의 </returns>
    public BonusStat GetStatus()
    {
        BonusStat returnStatus = new BonusStat();
        foreach (var skill in _gainedSkill)
        {
            if (!skill.isConditional)
            {
                returnStatus += skill.status;
            }
        }
        return returnStatus;
    }

    public void GetGold(int gold) => Gold += gold;
    public void UseGold(int gold) => Gold -= gold;
}
