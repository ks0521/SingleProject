using System;
using Base.PoolSO;
using Base.Utilities;
using UnityEngine;

namespace SO.Mech
{
    public class MechHealth : MonoBehaviour, IHittable
    {
        public MechStatus _stat;
        private MechBehavior _Behavior;
        public event Action<int, int> OnHpChanged;
        public event Action<float> OnHitStopped;
        public event Action<MechHealth> OnDie;
        private bool _isDead;
        [SerializeField]private int _maxHp;
        public int MaxHp => _maxHp;
        [SerializeField]private int _hp;
        public int Hp
        {
            get => _hp;
            private set
            {
                _hp = value;
                //Debug.Log($"Now Hp {value}");
                if (_hp <= 0 && !_isDead) //사망판정 중복 방지용
                {
                    Die();
                }
                OnHpChanged?.Invoke(Hp,_maxHp);
            }
        }

        public void Init(int hp, int maxHp)
        {
            _hp = hp;
            _maxHp = maxHp;
        }
        private void Awake()
        {
            _Behavior = GetComponent<MechBehavior>();
            _stat = GetComponent<MechStatus>();
            if (_stat is null)
            {
                Debug.Log("MechHealth : 초기 스탯이 입력되지 않았습니다. ");
            }
        }

        private void OnEnable()
        {   
            _maxHp = (_stat.archeType is null) ? 100 : _stat.archeType.mechBaseStatus.maxHp;
            _hp = _maxHp;
            _isDead = false;
        }

        public void Refresh() => OnHpChanged?.Invoke(_hp, _maxHp);
        private void Start()
        {
            OnHpChanged?.Invoke(Hp,_maxHp);
        }

        public void Die()
        {
            _isDead = true; 
            Debug.Log($"{gameObject.name} Die");
            OnDie?.Invoke(this);
            GetComponent<PooledObject>()?.Return();
        }
        /// <summary> 피격판정이 발생하는곳으로, 피해를 입고 피해경직 상태를 실행시킴</summary>
        public float Hit(float damage)
        {
            Hp -= (int)damage;
            _Behavior?.HitStop(0.15f);
            OnHitStopped?.Invoke(0.15f);
            return 0;
        }
    }
}