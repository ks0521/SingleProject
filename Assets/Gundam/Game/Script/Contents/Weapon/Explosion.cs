using System;
using System.Threading;
using Base.PoolSO;
using Base.Utilities;
using Contents.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Contents.Weapon
{
    public enum ExplosionType
    {
        Decorate, //이펙트용
        Damaging //피해용
    }

    public class Explosion : MonoBehaviour
    {
        private FinalStat _stat; //최종적인 폭발의 스텟(피해량과 범위)
        private PlayerAttackFeedback _ownerFeedback; //플레이어에게 공격결과 전달 컴포넌트
        private PooledObject _pooledObject; //풀링된 오브젝트용 스크립트(풀 반환용)
        [SerializeField]private int _targetLayerMask;
        private float _baseRadius = 1.3f;
        private float _finalRadius;
        [FormerlySerializedAs("explosion")] public ExplosionType expType;

        private void Awake()
        {
            _pooledObject = GetComponentInParent<PooledObject>();
        }

        public void Init(int parentLayer,in FinalStat stat, PlayerAttackFeedback owner)
        {
            _stat = stat;
            _ownerFeedback = owner; //owner가 null인 경우도 존재(owner가 AI)
            //폭발 범위와 실제 스케일 조정
            _finalRadius = _baseRadius * (1 + stat.AttackRadius);
            float scale = _finalRadius / _baseRadius;
            transform.localScale = Vector3.one * scale;

            //자신의 레이어를 조정해서 아군 팀에게 피해입히지 않게 조치
            gameObject.layer = (parentLayer == (int)GameLayer.AllyAttack)
                ? (int)GameLayer.AllyAttack
                : (int)GameLayer.EnemyAttack;
            _targetLayerMask = (parentLayer == (int)GameLayer.AllyAttack)
                ? 1<<(int)GameLayer.Enemy
                : 1<<(int)GameLayer.Ally;
            Exp();
        }

        void Exp()
        {
            if (expType == ExplosionType.Damaging)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, _finalRadius,_targetLayerMask);
                Debug.Log($"Explosion, {hits.Length} has hit");

                foreach (var target in hits)
                {
                    Debug.Log($"{target.name} layer={target.gameObject.layer}");
                    if (target.TryGetComponent(out IHittable hitter))
                    {
                        Debug.Log("히트 성공");
                        hitter.Hit(_stat.Damage);
                        _ownerFeedback?.AttackSuccess((int)_stat.Damage);
                    }
                }
            }
            Return(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, _finalRadius);
        }

        async UniTaskVoid Return(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.8f), cancellationToken: token);
            _pooledObject.Return();
        }
    }
}