using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Contents.Weapon
{
    [CreateAssetMenu]
    public class WeaponData : ScriptableObject
    {
        [Header("Common Status")]
        public int fireRate;
        public float damage;
        public AttackType AttackType;

        [Header("ProjectileType Only")] public Projectile ProjectileStat;
        [Header("Raycast Only")] public Raycast RaycastStat;
    }
    [Serializable]
    public struct Projectile
    {
        public float speed;
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