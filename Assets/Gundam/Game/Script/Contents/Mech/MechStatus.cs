using System;
using System.Collections;
using System.Collections.Generic;
using Contents.Mech;
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
    public float increseSpeed; // 속도 증가
    public float increseDamageReduction; //피해 감소
    public float multipleDamageReduction; //피해 배율

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
        };
    }
}
public class MechStatus : MonoBehaviour
{
    public BonusStat RuntimeBonusStat;
    public BaseStatueSO _baseStatue;
    public List<PassiveSkillSO> mechSkills;
    private void Awake()
    {
        RuntimeBonusStat = new BonusStat();
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
