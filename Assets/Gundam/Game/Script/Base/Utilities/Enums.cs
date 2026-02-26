using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 공격 종류 - 단일 / 폭발 / 히트스캔 </summary>
public enum AttackType
{
    Proj_DirectHit, //단일 투사체
    Proj_Explosion, //폭발
    Raycast, //레이저
    Melee //근접공격
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