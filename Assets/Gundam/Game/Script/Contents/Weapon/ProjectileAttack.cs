using System;
using System.Threading;
using Base.Manager.Test;
using Base.PoolSO;
using Base.Utilities;
using SO.Mech;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using SO.Player;
using UnityEditor.Rendering;
using UnityEngine.Serialization;


public enum GameLayer
{
    Default = 0,
    UI = 5,
    Ground = 6,
    Crosshair = 7,
    Ally = 8, //아군의 콜라이더
    AllyAttack = 9, //플레이어 및 아군의 공격 투사체 + 레이캐스트
    Player = 10, //플레이어 콜라이더
    Enemy = 11, // 적의 콜라이더
    EnemyAttack = 12 // 적의 공격 투사체 + 레이캐스트
}

namespace SO.Weapon
{
    

    /// <summary> 투사체의 이동을 담당하는 클래스</summary>
    public class ProjectileAttack : MonoBehaviour
    {
        private Rigidbody _rb;
        [SerializeField] private WeaponData _weaponData;
        [SerializeField] private int _hitterLayer; //피해를 입힐 대상
        [SerializeField] private FinalStat _finalStat;
        private Collider _myCollider;
        private Collider _ownerCollider;
        private CancellationTokenSource _token;
        private PlayerAttackFeedback _ownerFeedback;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _myCollider = GetComponent<Collider>();
            _finalStat = new FinalStat();
        }

        private void OnEnable()
        {
            if (_weaponData is null)
            {
                Debug.LogWarning("WeaPonData is Null");
            }
            _token = new CancellationTokenSource();
        }

        public void Init(Collider owner, Vector3 dir, FinalStat stat)
        {
            SetParent(owner);
            SetStat(dir, stat);
            SetTeam((GameLayer)owner.gameObject.layer);
            TimeOut(_token.Token).Forget();
        }
        //지금은 같은 진영의 투사체 판정이 레이어에서 조절되서 사용은 X
        public void SetParent(Collider owner)
        {
            owner.gameObject.TryGetComponent(out _ownerFeedback);
            _ownerCollider = owner;
            Physics.IgnoreCollision(_ownerCollider, _myCollider);
        }

        public void SetTeam(GameLayer myTeam)
        {
            //Debug.Log(myTeam);
            switch (myTeam)
            {   
                case GameLayer.Ally:
                    gameObject.layer = (int)GameLayer.AllyAttack;
                    break;
                case GameLayer.AllyAttack:
                    gameObject.layer = (int)GameLayer.AllyAttack;
                    break;
                case GameLayer.Enemy:
                    gameObject.layer = (int)GameLayer.EnemyAttack;
                    break;
                case GameLayer.EnemyAttack:
                    gameObject.layer = (int)GameLayer.EnemyAttack;
                    break;
                default:
                    Debug.LogWarning("중립오브젝트이거나 레이어 설정이 잘못되었습니다");
                    break;
            }
            //Debug.Log((GameLayer)gameObject.layer);
        }

        public void SetStat(Vector3 dir,in FinalStat stat)
        {
            _finalStat = stat;
            _rb.velocity = dir * _weaponData.projectileStat.speed;
            //기타 추가 스펙(각 투사체별 특징 작성...)
        }

        async UniTaskVoid TimeOut(CancellationToken token)
        {
            Debug.Log($"timeout : {_finalStat.Range}sec");
            await UniTask.Delay(TimeSpan.FromSeconds(_finalStat.Range),
                cancellationToken: token);
            Debug.Log("시간초과 ");
            GetComponent<PooledObject>()?.Return();
        }

        private void OnCollisionEnter(Collision other)
        {
            //Debug.Log("충돌");
            other.gameObject.TryGetComponent(out IHittable hitTarget);
            //단일타격이고 충돌 대상이 피해를 입을 수 있을 때 실행
            if (_weaponData.attackType == AttackType.Proj_DirectHit && hitTarget is not null)
            {
                hitTarget.Hit(_finalStat.Damage);
                _ownerFeedback?.AttackSuccess((int)_finalStat.Damage);
            }
            Explosion();
            GetComponent<PooledObject>()?.Return();
        }

        private void Explosion()
        {
            //단일 공격이면 피격위치에 장식용 VFX 생성
            if (_weaponData.attackType == AttackType.Proj_DirectHit)
            {
                PoolManager.poolDic[_weaponData.projectileStat.explosion]
                           .UsePool(transform.position, Quaternion.identity);
            }
            //폭발형 공격이면 피격위치에 폭발 VFX생성 후 Init
            else
            {
                Explosion explosion = PoolManager.poolDic[_weaponData.projectileStat.explosion]
                           .UsePool(transform.position, Quaternion.identity).GetComponent<Explosion>();
                explosion.Init(gameObject.layer, _finalStat, _ownerFeedback);
            }
        }
        private void OnDisable()
        {
            _token.Cancel();
            _token.Dispose();
            //풀로 반환될때 기존 ignore 무시 (다음에 누가쓸지 모름)
            if (_ownerCollider != null)
            {
                Physics.IgnoreCollision(_ownerCollider, _myCollider, false);
            }
        }
    }
}