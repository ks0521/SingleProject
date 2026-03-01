using System.Collections;
using System.Collections.Generic;
using Base.UI.WeaponSlot;
using SO.Weapon;
using UnityEditor.Compilation;
using UnityEngine;

namespace SO.Mech
{
    public enum MechRangeType { Short,Middle,Long }
    /// <summary> NPC의 행동 판단 파라미터</summary>
    [CreateAssetMenu(menuName = "Mech/AIParameter")]
    public class AIParameter : ScriptableObject
    {
        [Header("Ranges")] 
        public float desiredRange = 5f; //선호 유지거리
        public float rangeBand = 1; //허용 오차값
        public float minSafeRange = 3.5f; //최소 안전거리(더 가까워지면 후퇴)

        [Header("Attack")] 
        public float attackRange = 6.5f; //공격 가능 거리
        public float attackBurstTime = 0.8f; //공격 상태 유지시간(해당시간동안 쭉 공격)
        public float attackCooldown = 0.2f; //공격상태(BurstTime) 사이 턴

        [Header("Reposition / Strafe")] 
        public float strafeHoldTime = 1.0f; //우회(좌우이동)시간
        public float strafeWeight = 1.0f; //우회(좌우이동) 비중
        public float rangeFixWeight = 0.6f; // 거리 보정 비중

        [Header("Tick")] public float decisionInterval = 0.15f; // 전이 주기
    }
}