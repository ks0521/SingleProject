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
        /// <summary>기체의 Controller에서 공격 가능한지 여부 확인</summary>
        /// <param name="aimData">에임 정보(타겟의 위치와 발사 방향)</param>
        /// <param name="curWeaponParts">현재 사용중인 무기</param>
        /// <param name="mechRuntimeStat">기체의 추가 스탯</param>
        /// <returns>WeaponParts에 공격입력이 들어갔으면 true, 안들어갔으면 false</returns>
        public bool AttackInvoke(in AimData aimData, in WeaponParts curWeaponParts, in MechRuntimeStatus mechRuntimeStat)
        {
            if (!curWeaponParts.CanShot)
            {
                Debug.Log("발사 쿨타임");
                return false;
            }
            else
            {
                curWeaponParts.Attack(aimData, mechRuntimeStat);
                return true;
            }
        }
    }
}