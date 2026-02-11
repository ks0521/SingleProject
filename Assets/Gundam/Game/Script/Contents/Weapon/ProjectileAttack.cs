using System;
using System.Threading;
using Base.Manager.Test;
using Base.PoolSO;
using Base.Utilities;
using Contents.Mech;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEditor.Rendering;

/// <summary> 해당 인터페이스는 오브젝트 풀에 저장되는 오브젝트에서 구현해야 한다(ReturnPool 구현에 필요) </summary>
public interface IObjectPooled
{
    public void SetReturnPoolKey(PoolID id);
}

/// <summary> 해당 인터페이스는 오브젝트 풀에서 꺼낼 때 팀 선택이 필요한 오브젝트(투사체나 기체)에서 사용해야한다</summary>
public interface ITeamSelectable
{
    public void SetTeam(GameLayer myTeam);
}

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

namespace Contents.Weapon
{
    public struct FinalStat
    {
        public float Damage;
        public float Speed;
        public float FireRate;
        [Header("Explosion Type Only")] public float ExplosionRadius;
    }

    /// <summary> 투사체의 이동을 담당하는 클래스</summary>
    public class ProjectileAttack : MonoBehaviour, IObjectPooled
    {
        private Rigidbody _rb;
        [SerializeField] private WeaponData _weaponData;
        [SerializeField] private PoolID _returnPoolKey;
        [SerializeField] private int _hitterLayer; //피해를 입힐 대상
        [SerializeField] private FinalStat _finalStat;
        private Collider _myCollider;
        private Collider _ownerCollider;
        private CancellationTokenSource _token;
        private IHittable _hitTarget;
        private float _damageRespond;
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
            TimeOut(_token.Token).Forget();
        }

        public void Init(Collider owner, Vector3 dir, MechRuntimeStatus stat)
        {
            SetParent(owner);
            SetStat(dir, stat);
            SetTeam((GameLayer)owner.gameObject.layer);
        }

        public void SetParent(Collider owner)
        {
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
            Debug.Log((GameLayer)gameObject.layer);
        }

        public void SetStat(Vector3 dir, MechRuntimeStatus stat)
        {
            _finalStat.Damage = _weaponData.damage + stat.increseDmg;
            _finalStat.FireRate = _weaponData.RPM;
            _finalStat.Speed = _weaponData.projectileStat.speed + stat.increseFireRate;
            _finalStat.ExplosionRadius = _weaponData.projectileStat.explosionRadius;
            _rb.velocity = dir * _finalStat.Speed;
        }

        public void SetReturnPoolKey(PoolID id)
        {
            _returnPoolKey = id;
        }

        async UniTaskVoid TimeOut(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(3f),
                cancellationToken: token);
            Debug.Log("시간초과 ");
            GetComponent<PooledObject>()?.Return();
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("충돌");
            if (other.gameObject.TryGetComponent(out _hitTarget))
            {
                _damageRespond = _hitTarget.Hit(_finalStat.Damage);
                //owner에 인터페이스(getDamage)같은거 넣고 얼만큼 피해를 가했는지 보내주면 
                //나중에 가한 피해량만큼 회복같은 기능을 구현할 수 있을듯함
            }
            GetComponent<PooledObject>()?.Return();
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