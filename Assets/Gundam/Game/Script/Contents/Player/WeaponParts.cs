using System.Collections;
using System.Collections.Generic;
using Base.Manager.Test;
using Contents.Weapon;
using UnityEngine;

public class WeaponParts : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private PoolID weaponId;
    [SerializeField] private Transform firePoint;

    private Vector3 targetPos;
    private Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Attack(MechStatus stat)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            targetPos = hit.point;
        }
        else
        {
            targetPos = ray.origin + ray.direction * 1000f;
        }

        dir = (targetPos - firePoint.position).normalized;
        if (weaponData.AttackType == AttackType.Raycast) //레이캐스트
        {
            //RaycastAttack()
        }
        else //투사체
        {
            GameObject obj = ObjectPoolGenericManager.poolDic[weaponId].UsePool
                (firePoint.position, firePoint.rotation);
            obj.GetComponent<ProjectileAttack>().Init(dir, stat);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}