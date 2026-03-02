using System;
using System.Threading;
using Base.Manager.Test;
using Base.Utilities;
using Contnts.Player;
using Cysharp.Threading.Tasks;
using SO.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace SO.Weapon
{
    /// <summary> 실제 공격시점에 적용되는 최종 정보</summary>
    [Serializable]
    public struct FinalStat
    {
        public float Damage; //피해량(전 무기)
        public float RPM; //연사력(전 무기)
        public float Interval; // 실제 사격 간 딜레이
        public int Ammo; // 장탄수(melee제외)
        public float ReloadingTime; //재장전 시간(melee제외)
        public float AttackRadius; //공격 범위(폭발과 raycast)
        public float Range; //사거리(raycast) / 투사체 이동 시간(projectile)
    }
    /// <summary> 각 무기 파츠에 붙어 실제 투사체 / 레이캐스트 공격을 실행하는 말단</summary>
    public class WeaponParts : MonoBehaviour
    {
#region Variable
        [Header("Components")]
        [SerializeField]private WeaponData weaponData; //생성되는 무기의 정보
        public WeaponData WeaponData => weaponData;
        [SerializeField]private FinalStat _finalStat; //공격간 사용되는 최종 스탯
        public Sprite weaponImg; // 무기 이미지
        private PlayerAttackFeedback _attackFeedback; //플레이어 피해 피드백 컴포넌트
        private Collider _parentCollider; //부모 콜라이더(Collider Ignore용)
        private MeleeComboAttack _meleeAttack; //근접공격용 컴포넌트
        private CancellationToken _token;
        
        private float curDelay; //현재 공격 후딜레이
        private float curReloading; //현재 재장전
        private float curAmmo; //현재 장탄수
        public bool IsReloading { get; private set; } //장전여부
        public bool IsDelay { get; private set; } //발사 대기 여부
        public Transform FirePoint { get; private set; } //무기의 발사위치

        private int _raycastMask; //레이캐스트 공격용 마스크
        private LineRenderer _lineRenderer; //레이캐스트 공격 반경 조정용
#endregion
        public float ReloadingRatio //현재 재장전 진행도
        {
            get
            {
                if (!IsReloading) return 1f;
                if (_finalStat.ReloadingTime <= 0f) return 1f; //0초 재장전이나 설정누락시 비율 1로 고정
                float ratio = 1f - (curReloading / _finalStat.ReloadingTime);
                return Mathf.Clamp01(ratio);
            }
        }
        public float AmmoRatio //잔탄 비율
        {
            get
            {
                if (_finalStat.Ammo <= 0) return 1f;
                float ratio = curAmmo / _finalStat.Ammo;
                return Mathf.Clamp01(ratio);
            }
        }

        private void Awake()
        {
            _token = this.GetCancellationTokenOnDestroy();
            _parentCollider = GetComponentInParent<Collider>();
            _attackFeedback = GetComponentInParent<PlayerAttackFeedback>();
            _lineRenderer = GetComponent<LineRenderer>();
            _meleeAttack = GetComponent<MeleeComboAttack>();
            FirePoint = GetComponentInChildren<FirePointMarker>().transform;
            if (FirePoint == null)
            {
                Debug.LogWarning($"{this.gameObject.name}부위 발사위치 탐색하지 못함");
            }
            //자기 레이어에 따라 피격대상 변경(ally : 장애물 + 적 / enemy : 장애물 + 아군)
            _raycastMask = (gameObject.layer == (int)GameLayer.Ally)
                ? 1 << (int)GameLayer.Default | 1 << (int)GameLayer.Enemy
                : 1 << (int)GameLayer.Default | 1 << (int)GameLayer.Ally;
            //gameObject.TryGetComponent(out _lineRenderer);
            StatusUpdate(new BonusStat());
        }

        /// <summary> WeaponParts 초기값 설정</summary>
        /// <param name="stat"></param>
        public void Init(BonusStat stat)
        {
            StatusUpdate(stat);
            curAmmo = _finalStat.Ammo;
            IsReloading = IsDelay = false;
        }
        /// <summary> 무기 스펙 업데이트</summary>
        /// <param name="stat"> 사용할 정보</param>
        public void StatusUpdate(BonusStat stat)
        {
            switch (weaponData.attackType)
            {
                case AttackType.Raycast:
                    _finalStat.Range = weaponData.raycastStat.range * (1 + stat.multipleRange);
                    break;
                case AttackType.Proj_DirectHit:
                    _finalStat.Range = weaponData.projectileStat.lifetime * (1 + stat.multipleRange);
                    break;
                case AttackType.Proj_Explosion:
                    _finalStat.Range = weaponData.projectileStat.lifetime * (1 + stat.multipleRange);
                    break;
            }
            _finalStat.Damage = (weaponData.damage + stat.increaseDmg) * (1 + stat.multipleDmg);
            _finalStat.RPM = weaponData.RPM * (1 + stat.increaseFireRate);
            _finalStat.Interval = (_finalStat.RPM>0)?60f / _finalStat.RPM : 0.5f; //RPM이 0일때 인터벌 0.5초로 고정
            _finalStat.Ammo = (int)((weaponData.ammo + stat.increaseAmmo) * (1 + stat.multipleAmmo));
            _finalStat.AttackRadius = weaponData.attackRadius * (1 + stat.multipleAttackRadius);
            _finalStat.ReloadingTime = weaponData.reloadingTime * (1 - stat.multipleReload);
        }
        /// <summary> 자신이 가지고 있는 WeaponData이용해서 공격</summary>
        /// <param name="aim"> 조준정보 </param>
        /// <param name="stat"> 공격 시점 추가스탯</param>
        public void Attack(in AimData aim,in BonusStat stat)
        {
            StatusUpdate(stat);
            if (weaponData.attackType == AttackType.Raycast)  //레이캐스트 공격
                RaycastAttack(aim);
            else if (weaponData.attackType == AttackType.Melee) //근접 공격
                MeleeAttack();
            else 
                ProjectileAttack(aim);
            //근접공격은 잔탄량 없고 MeleeAttack 내에서 딜레이 조절
            if (weaponData.attackType != AttackType.Melee)  
            {
                if (--curAmmo <= 0)
                    Reloading(_token, _finalStat.ReloadingTime).Forget();
                else
                    AttackDelay(_token,_finalStat.Interval).Forget();
            }
        }
        /// <summary> 레이캐스트 타입의 공격을 한다 </summary>
        /// <param name="aimData"> 조준 정보 </param>
        void RaycastAttack(in AimData aimData)
        {
            if (_lineRenderer is null)
            {
                Debug.LogWarning("레이캐스트 공격타입이지만 라인 렌더러 컴포넌트가 없습니다");
                return;
            }
            
            Ray ray = new Ray(FirePoint.position, aimData.dir);
            _lineRenderer.SetPosition(0,FirePoint.position);
            _lineRenderer.startWidth = _finalStat.AttackRadius;
            _lineRenderer.endWidth = _finalStat.AttackRadius;
            
            //레이캐스트 맞은 대상이 있으면 linerenderer의 끝점을 해당 위치로 조정
            if (Physics.SphereCast(ray, _finalStat.AttackRadius, out RaycastHit hit, _finalStat.Range, _raycastMask))
            {
                if (hit.collider.gameObject.TryGetComponent(out IHittable hitter))
                {
                    hitter.Hit(_finalStat.Damage);
                    _attackFeedback?.AttackSuccess((int)_finalStat.Damage);
                }
                _lineRenderer.SetPosition(1,hit.point);
            }
            //대상이 없으면 linerenderer의 끝점을 계산해서 지정
            else
            {
                Vector3 endpos = ray.origin + ray.direction * _finalStat.Range;
                _lineRenderer.SetPosition(1,endpos);
            }
            LaserRendering(this.GetCancellationTokenOnDestroy()).Forget();
            Debug.DrawRay(FirePoint.position,aimData.dir *1000,Color.red,0.1f ); //디버그용
        }
        /// <summary> 레이캐스팅 무기 발사했을때 이펙트 출력 </summary>
        /// <param name="token"></param>
        async UniTaskVoid LaserRendering(CancellationToken token)
        {
            _lineRenderer.enabled = true;
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: token);
            _lineRenderer.enabled = false;
        }
        /// <summary> 투사체 타입의 공격을 한다 </summary>
        /// <param name="aim"> 조준 정보 </param>
        void ProjectileAttack(in AimData aim)
        {
            GameObject obj = PoolManager.poolDic[weaponData.ID].UsePool
                (FirePoint.position, FirePoint.rotation);
            if (obj.TryGetComponent<ProjectileAttack>(out var projectileBullet))
            {
                Debug.Log("projAttack");
                projectileBullet.Init(_parentCollider, aim.dir, _finalStat);
            }
            else //투사체가 아닌 이상한게 나왔을때
            {
                Debug.LogError("풀에서 잘못된 오브젝트를 뽑아옴");
                PoolManager.poolDic[weaponData.ID].ReturnPool(obj);
            }
        }
        /// <summary> 근접공격을 한다 </summary>
        /// <param name="aim"> 조준정보</param>
        void MeleeAttack()
        {
            if (_meleeAttack is null) return; 

            // 첫 공격때만 MeleeAttack 관리, 연속공격은 MeleeComboAttack에서 입력받음
            if (!_meleeAttack.IsPlaying)
                _meleeAttack.StartCombo(_finalStat);
        }
        /// <summary> 공격 간 딜레이 입력차단</summary>
        /// <param name="duration"> 딜레이 시간 </param>
        async UniTaskVoid AttackDelay(CancellationToken token,float duration)
        {
            IsDelay = true;
            curDelay = duration;
            while (curDelay > 0)
            {
                curDelay -= Time.deltaTime;
                await UniTask.Yield(cancellationToken: token);
            }
            IsDelay = false;
        }
        /// <summary> 무기 재장전 간 입력차단</summary>
        /// <param name="duration">재장전 시간</param>
        async UniTaskVoid Reloading(CancellationToken token,float duration)
        {
            IsReloading = true;
            curReloading = duration;
            while (curReloading > 0)
            {
                curReloading -= Time.deltaTime;
                await UniTask.Yield(cancellationToken: token);
            }
            curAmmo = _finalStat.Ammo;
            IsReloading = false;
            
        }
    }
}