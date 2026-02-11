using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Contents.Mech
{
    public enum AniMove
     {
         Idle = 0,
         Walk = 1,
         Booster = 2,
         Jump = 3
     }
    /// <summary> 각 기체의 추가 스탯</summary>
    [Serializable]
    public struct MechRuntimeStatus
    {
        public int increseDmg;
        public int multipleDmg;
        public int increseFireRate;
        public int multipleFireRate;
        public GameLayer mechTeam;
    }
}