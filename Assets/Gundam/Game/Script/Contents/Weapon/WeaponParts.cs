using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Base.Manager.Test;
using Contents.Mech;
using Contnts.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Contents.Weapon
{
    /// <summary> 각 무기 파츠에 붙어 실제 투사체 / 레이캐스트 공격을 실행하는 말단</summary>
    public class WeaponParts : MonoBehaviour
    {
        private CancellationToken _token;
        [SerializeField] private WeaponData weaponData;
        public float FireDelay { get; private set; }
        public Transform FirePoint { get; private set; }
        public bool CanShot { get; private set; }

        private void Awake()
        {
            if (weaponData.RPM == 0) FireDelay = 1f; //rpm이 없으면 60RPM으로 가정
            else FireDelay = 60f / weaponData.RPM; //발사간격 : 60(초) / RPM(분당 발사수)
            _token = this.GetCancellationTokenOnDestroy();
            CanShot = true;
            FirePoint = GetComponentInChildren<FirePointMarker>().transform;
            if (FirePoint == null)
            {
                Debug.LogWarning($"{this.gameObject.name}부위 발사위치 탐색하지 못함");
            }
        }

        public void Attack(AimData data, MechStatus stat)
        {
            if (weaponData.attackType == AttackType.Raycast)
            {
                RaycastAttack();
            }
            else
            {
                GameObject obj = PoolManager.poolDic[weaponData.projectileID].UsePool
                    (FirePoint.position, FirePoint.rotation);
                if (obj.TryGetComponent<ProjectileAttack>(out var projectileBullet))
                {
                    projectileBullet.Init(GetComponentInParent<Collider>(), data.dir, stat);
                }
                else
                {
                    Debug.LogError("풀에서 잘못된 오브젝트를 뽑아옴");
                    PoolManager.poolDic[weaponData.projectileID].ReturnPool(obj);
                }
            }

            AttackDelay(_token,FireDelay).Forget();
        }

        async UniTaskVoid AttackDelay(CancellationToken token,float duration)
        {
            CanShot = false;
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
            CanShot = true;
        }
        void RaycastAttack()
        {
            Debug.Log("레이캐스트 공격");
        }
    }
}