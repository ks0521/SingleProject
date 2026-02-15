using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Base.Manager.Test;
using Contents.Mech;
using Contnts.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Contents.Weapon
{
    /// <summary> 각 무기 파츠에 붙어 실제 투사체 / 레이캐스트 공격을 실행하는 말단</summary>
    public class WeaponParts : MonoBehaviour
    {
        private CancellationToken _token;
        [SerializeField] private WeaponData weaponData;

        public Sprite weaponImg;
        private Collider _parentCollider;
        private float entireCoolDown; //전체 쿨타임
        private float curCoolDown; //현재 쿨타임

        public float CoolDownRatio
        {
            get
            {
                if (CanShot) return 1;
                return Mathf.Clamp01((entireCoolDown - curCoolDown) / entireCoolDown);
            }
        }
        public float FireDelay { get; private set; }
        public Transform FirePoint { get; private set; }
        public bool CanShot { get; private set; }

        private void Awake()
        {
            if (weaponData.RPM == 0) FireDelay = 1f; //rpm이 없으면 60RPM으로 가정
            else FireDelay = 60f / weaponData.RPM; //발사간격 : 60(초) / RPM(분당 발사수)
            _token = this.GetCancellationTokenOnDestroy();
            CanShot = true;
            _parentCollider = GetComponentInParent<Collider>();
            FirePoint = GetComponentInChildren<FirePointMarker>().transform;
            if (FirePoint == null)
            {
                Debug.LogWarning($"{this.gameObject.name}부위 발사위치 탐색하지 못함");
            }
        }

        public void Attack(AimData data, MechRuntimeStatus stat)
        {
            if (weaponData.attackType == AttackType.Raycast)
            {
                RaycastAttack();
            }
            else
            {
                GameObject obj = PoolManager.poolDic[weaponData.ID].UsePool
                    (FirePoint.position, FirePoint.rotation);
                if (obj.TryGetComponent<ProjectileAttack>(out var projectileBullet))
                {
                    projectileBullet.Init(_parentCollider, data.dir, stat);
                }
                else
                {
                    Debug.LogError("풀에서 잘못된 오브젝트를 뽑아옴");
                    PoolManager.poolDic[weaponData.ID].ReturnPool(obj);
                }
            }

            entireCoolDown = FireDelay;
            AttackDelay(_token,entireCoolDown).Forget();
        }

        async UniTaskVoid AttackDelay(CancellationToken token,float duration)
        {
            CanShot = false;
            curCoolDown = duration;
            while (curCoolDown > 0)
            {
                curCoolDown -= Time.deltaTime;
                await UniTask.Yield(cancellationToken: token);
            }
            CanShot = true;
        }
        void RaycastAttack()
        {
            Debug.Log("레이캐스트 공격");
        }
    }
}