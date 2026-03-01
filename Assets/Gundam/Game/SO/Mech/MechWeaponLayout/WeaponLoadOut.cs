using System.Collections;
using System.Collections.Generic;
using SO.Weapon;
using SO.Mech;
using UnityEngine;

namespace SO.Mech
{
    /// <summary> 기체의 무기 로드아웃
    /// NPC기체는 사용할 수 있는 무기의 후보군, 플레이어는 현재 보유중인 무기를 저장한다</summary>
    [CreateAssetMenu(menuName = "Mech/Weapons Loadout")]
    public class WeaponLoadOut : ScriptableObject
    {
        public List<WeaponData> weapons;
    }
}