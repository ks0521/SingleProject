using System.Collections;
using System.Collections.Generic;
using SO.Mech;
using UnityEngine;

namespace SO.Weapon
{
    public abstract class WeaponAttack : MonoBehaviour
    {
        public abstract void SetReturnPoolKey(PoolID id);
        public abstract void Init(BonusStat stat);
    }
}