using System;
using System.Collections;
using System.Collections.Generic;
using Base.Utilities;
using Contents.Player;
using Contents.Weapon;
using Contnts.Player;
using UnityEngine;

namespace Contents.Mech
{
    /// <summary> 무기발사 조건(발사 딜레이 / 차지 / 탄알 잔량 등..)을 확인하고 가능하면 MechBehavior에
    /// Attack요청을 보냄</summary>
    public class AttackInvoker : MonoBehaviour
    {
        [SerializeField] private MechBehavior behavior;
        /// <summary> 기체의 Controller에서 공격 가능한지 여부 확인 </summary>
        /// <param name="aimData"></param>
        /// <param name="curWeaponParts"></param>
        /// <param name="mechStat"></param>
        public bool AttackInvoke(in AimData aimData, in WeaponParts curWeaponParts, in MechStatus mechStat)
        {
            if (!curWeaponParts.CanShot)
            {
                Debug.Log("발사 쿨타임");
                return false;
            }
            else
            {
                curWeaponParts.Attack(aimData, mechStat);
                return true;
            }
        }
    }
}