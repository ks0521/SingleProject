using System;
using System.Collections;
using System.Collections.Generic;
using Contents.Mech;
using Contents.Player;
using Contents.Weapon;
using UnityEngine;
using UnityEngine.Serialization;


/// <summary> 각 기체의 추가 스탯</summary>
[Serializable]
public struct BonusStat
{
    public float increseDmg; //데미지 증가
    public float multipleDmg; //데미지 배율 증가
    public float increseFireRate; // 연사속도 증가
    public float multipleFireRate; // 연사 배율 증가
    public float multipleRange; //raycast 무기 사거리
    public float increseAmmo; //탄창 증가
    public float multipleAmmo; //탄창 배율 증가
    public float multipleReload; //재장전 시간 배율 감소
    public float increseSpeed; // 속도 증가
    public float increseDamageReduction; //피해 감소
    public float multipleDamageReduction; //피해 감소 배율
    public float multipleAttackRadius; //공격 범위(폭발 반경 / 빔 굵기)
    public static BonusStat operator +(BonusStat a, BonusStat b)
    {
        return new BonusStat
        {
            increseDmg = a.increseDmg + b.increseDmg,
            multipleDmg = a.multipleDmg + b.multipleDmg,
            increseFireRate = a.increseFireRate + b.increseFireRate,
            multipleFireRate = a.multipleFireRate + b.multipleFireRate,
            increseSpeed = a.increseSpeed + b.increseSpeed,
            increseDamageReduction = a.increseDamageReduction + b.increseDamageReduction,
            multipleDamageReduction = a.multipleDamageReduction + b.multipleDamageReduction,
            multipleAttackRadius = a.multipleAttackRadius + b.multipleAttackRadius,
            increseAmmo = a.increseAmmo + b.increseAmmo,
            multipleAmmo = a.multipleAmmo + b.multipleAmmo,
            multipleReload = a.multipleReload + b.multipleReload
        };
    }
}
public class MechStatus : MonoBehaviour
{
    public BonusStat RuntimeBonusStat;
    public BaseStatueSO _baseStatue;
    public PlayerWeaponManager _WeaponManager;
    public List<PassiveSkillSO> mechSkills;
    public bool SuperArmor { get; private set; } //경직면역여부
    private void Awake()
    {
        RuntimeBonusStat = new BonusStat();
        WeaponParts[] weapons = GetComponentsInChildren<WeaponParts>();
        //초기 상시형 패시브 적용
        foreach (var weapon in weapons)
        {
            weapon.Init(RuntimeBonusStat);
        }
    }
    
    public void SuperArmorStatus(bool value)
    {
        SuperArmor = value;
    }
    /// <summary> 데미지 수치변경</summary>
    public void DamageChange(int value)
    {
        
    }
    /// <summary> 데미지 배율 변경</summary>
    public void DamageChange(float multiplier)
    {
        
    }
}
