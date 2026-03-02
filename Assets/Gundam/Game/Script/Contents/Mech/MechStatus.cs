using System;
using System.Collections;
using System.Collections.Generic;
using SO.Player;
using SO.Weapon;
using SO.Mech;
using UnityEngine;
using UnityEngine.Serialization;


/// <summary> 각 기체의 추가 스탯</summary>
[Serializable]
public struct BonusStat
{
    public float increaseDmg; //데미지 증가
    public float multipleDmg; //데미지 배율 증가
    public float increaseFireRate; // 연사속도 증가
    public float multipleFireRate; // 연사 배율 증가
    public float multipleRange; //raycast 무기 사거리
    public float increaseAmmo; //탄창 증가
    public float multipleAmmo; //탄창 배율 증가
    public float multipleReload; //재장전 시간 배율 감소
    public float increaseSpeed; // 속도 증가
    public float increaseDamageReduction; //피해 감소
    public float multipleDamageReduction; //피해 감소 배율
    public float multipleAttackRadius; //공격 범위(폭발 반경 / 빔 굵기)
    public float increaseHp; //추가 HP
    public static BonusStat operator +(BonusStat a, BonusStat b)
    {
        return new BonusStat
        {
            increaseDmg = a.increaseDmg + b.increaseDmg,
            multipleDmg = a.multipleDmg + b.multipleDmg,
            increaseFireRate = a.increaseFireRate + b.increaseFireRate,
            multipleFireRate = a.multipleFireRate + b.multipleFireRate,
            increaseSpeed = a.increaseSpeed + b.increaseSpeed,
            increaseDamageReduction = a.increaseDamageReduction + b.increaseDamageReduction,
            multipleDamageReduction = a.multipleDamageReduction + b.multipleDamageReduction,
            multipleAttackRadius = a.multipleAttackRadius + b.multipleAttackRadius,
            increaseAmmo = a.increaseAmmo + b.increaseAmmo,
            multipleAmmo = a.multipleAmmo + b.multipleAmmo,
            multipleReload = a.multipleReload + b.multipleReload,
            increaseHp = a.increaseHp + b.increaseHp,
        };
    }
}
public class MechStatus : MonoBehaviour
{
    public BonusStat runtimeBonusStat;
    public MechArcheTypeSO archeType;
    public List<PassiveSkillSO> mechSkills;
    public bool SuperArmor { get; private set; } //경직면역여부
    private void Awake()
    {
        WeaponParts[] weapons = GetComponentsInChildren<WeaponParts>();
        //초기 상시형 패시브 적용
        foreach (var weapon in weapons)
        {
            weapon.Init(runtimeBonusStat);
        }
    }
    public void Init(BonusStat startStat, MechArcheTypeSO archeTypeSo)
    {
        runtimeBonusStat = startStat;
        archeType = archeTypeSo;
    }
    public void SuperArmorStatus(bool value)
    {
        SuperArmor = value;
    }
}
