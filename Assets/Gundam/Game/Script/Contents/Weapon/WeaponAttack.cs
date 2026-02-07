using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Contents.Weapon
{
    public abstract class WeaponAttack : MonoBehaviour
    {
        public abstract void SetReturnPoolKey(PoolID id);
        public abstract void Init(MechStatus stat);
    }
}