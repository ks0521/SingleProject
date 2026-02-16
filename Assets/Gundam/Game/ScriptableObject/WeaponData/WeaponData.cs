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
        [FormerlySerializedAs("fireRate")] [Header("Common Status")]
        public int RPM;
        public float damage;
        public AttackType attackType;

        [Header("ProjectileType Only")] 
        public PoolID ID;
        public Projectile projectileStat;
        [Header("Raycast Only")] public Raycast raycastStat;
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