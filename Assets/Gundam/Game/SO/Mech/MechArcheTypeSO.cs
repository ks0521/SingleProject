using System.Collections;
using System.Collections.Generic;
using SO.Mech;
using UnityEngine;

namespace SO.Mech
{
    public enum PlayableType{ Npc, Player}
    /// <summary> 기체의 SO 허브 </summary>
    [CreateAssetMenu(menuName = "Mech/ArcheType")]
    public class MechArcheTypeSO : ScriptableObject
    {
        public MechRangeType rangeType; //기체 타입
        public PlayableType playable; //플레이어인지, npc인지 구분
        public MechBaseStatusSO mechBaseStatus; //기본 스탯
        public WeaponLoadOut weaponLoadOut; // 각 기체별 무기 후보군
        public AIParameter parameter; //플레이어는 없음
    }
}