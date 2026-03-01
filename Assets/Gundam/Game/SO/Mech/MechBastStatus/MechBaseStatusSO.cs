using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SO.Mech
{
    /// <summary> 기체의 기본 스탯</summary>
    [CreateAssetMenu(menuName = "Mech/Base Status")]
    public class MechBaseStatusSO : ScriptableObject
    {
        public float walkSpeed;
        public float runSpeed;
        public int maxHp;
    }
}