using System;
using System.Collections;
using System.Collections.Generic;
using Contents.Mech;
using UnityEngine;


/// <summary> 각 기체의 추가 스탯</summary>
[Serializable]
public struct MechRuntimeStatus
{
    public int increseDmg;
    public int multipleDmg;
    public int increseFireRate;
    public int multipleFireRate;
    public float increseSpeed;
    public GameLayer mechTeam;
}
public class MechStatus : MonoBehaviour
{
    public MechRuntimeStatus RuntimeStatus;
    public BaseStatueSO _baseStatue;
    private void Awake()
    {
        RuntimeStatus = new MechRuntimeStatus();
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
