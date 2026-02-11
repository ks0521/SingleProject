using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 공격 종류 - 단일 / 폭발 / 히트스캔 </summary>
public enum AttackType
{
    Proj_DirectHit,
    Proj_Explosion,
    Raycast
}

public enum ObjectPoolType
{
    MG_Bullet,
    MG_Shell,
    Canon_Bullet,
    Canon_Shell,
    SRM,
    LRM
}