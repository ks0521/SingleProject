using System.Collections;
using System.Collections.Generic;
using Contents.Mech;
using UnityEngine;

namespace Contents.Weapon
{
    public abstract class WeaponAttack : MonoBehaviour
    {
        public abstract void SetReturnPoolKey(PoolID id);
        public abstract void Init(MechRuntimeStatus stat);
    }
}