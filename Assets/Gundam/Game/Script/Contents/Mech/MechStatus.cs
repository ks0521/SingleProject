using System;
using System.Collections;
using System.Collections.Generic;
using Contents.Mech;
using UnityEngine;

public class MechStatus : MonoBehaviour
{
    public MechRuntimeStatus RuntimeStatus;

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
