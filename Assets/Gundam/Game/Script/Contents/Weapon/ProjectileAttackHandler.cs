using System;
using System.Collections;
using System.Collections.Generic;
using Base.Manager.Test;
using Contnts.Player;
using SO.Weapon;
using UnityEngine;

[Serializable]
public class ProjectileAttackHandler : MonoBehaviour,IWeaponHandler
{
    private WeaponData weaponData;
    private Transform FirePoint;
    private Collider _parentCollider;
    public void Init(WeaponData initWeapon)
    {
        weaponData = initWeapon;
        FirePoint = GetComponentInChildren<FirePointMarker>().transform;
        _parentCollider = GetComponentInParent<Collider>();
    }
    public void Attack(in AimData aim, in FinalStat stat)
    {
        GameObject obj = PoolManager.poolDic[weaponData.ID].UsePool
            (FirePoint.position, FirePoint.rotation);
        if (obj.TryGetComponent<ProjectileAttack>(out var projectileBullet))
        {
            Debug.Log("projAttack_IWeaponHandler");
            projectileBullet.Init(_parentCollider, aim.dir, stat);
        }
        else //투사체가 아닌 이상한게 나왔을때
        {
            Debug.LogError("풀에서 잘못된 오브젝트를 뽑아옴");
            PoolManager.poolDic[weaponData.ID].ReturnPool(obj);
        }
    }
}
