using System;
using UnityEngine;

namespace SO.Weapon
{
    [CreateAssetMenu(menuName = "Mech/Weapon/WeaponData",order = 1)]
    public class WeaponData : ScriptableObject
    {
        [Header("Common Status")]
        public AttackType attackType; //공격 종류
        public float damage; //피해량
        public int RPM; //분당 발사수
        public int ammo; //장탄수
        public float reloadingTime; // 재장전 시간
        public float attackRadius; //공격 범위
        [Header("ProjectileType Only")] 
        public PoolID ID; //풀 ID
        public Projectile projectileStat; //투사체 무기 스펙
        [Header("Raycast Only")] 
        public Raycast raycastStat; //레이캐스트 무기 스펙
    }
    [Serializable]
    public struct Projectile
    {
        public float lifetime; //투사체 유지시간(사거리)
        public float speed;
        public PoolID explosion;
    }
    [Serializable]
    public struct Raycast
    {
        public float range; //빔 범위
        public bool canPierce;
    }
}