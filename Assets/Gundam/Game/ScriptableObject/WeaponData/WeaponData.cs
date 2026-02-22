using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Contents.Weapon
{
    [CreateAssetMenu]
    public class WeaponData : ScriptableObject
    {
        [Header("Common Status")]
        public int RPM; //분당 발사수
        public float damage; //피해량
        public int ammo; //장탄수
        public AttackType attackType; //공격 종류
        
        [Header("ProjectileType Only")] 
        public PoolID ID; //풀 ID
        public Projectile projectileStat; //투사체 무기 스펙 
        [Header("Raycast Only")] public Raycast raycastStat; //레이캐스트 무기 스펙
    }
    [Serializable]
    public struct Projectile
    {
        public float speed;
        public PoolID explosion;
        [Header("Explosion Type Only")] 
        public float explosionRadius;
    }
    [Serializable]
    public struct Raycast
    {
        public float range;
        public bool canPierce;
    }
}