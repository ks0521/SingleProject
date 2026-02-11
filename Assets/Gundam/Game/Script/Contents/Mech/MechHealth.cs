using System;
using System.Collections;
using System.Collections.Generic;
using Base.PoolSO;
using Base.Utilities;
using Contents.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Contents.Mech
{
    public class MechHealth : MonoBehaviour, IHittable
    {
        public MechStatus _stat;
        private MechBehavior _Behavior;
        private bool isDead;
        private float maxHp = 100;
        private float hp = 100;
        public float HP
        {
            get => hp;
            private set
            {
                hp = value;
                Debug.Log($"Now Hp {value}");
                if (hp < 0 && !isDead) //사망판정 중복 방지용
                {
                    Die();
                }
            }
        }

        private void Awake()
        {
            _Behavior = GetComponent<MechBehavior>();
            _stat = GetComponent<MechStatus>();
        }

        private void OnEnable()
        {
            hp = maxHp;
            isDead = false;
        }

        public void Die()
        {
            isDead = true; 
            Debug.Log($"{gameObject.name} Die");
            GetComponent<PooledObject>()?.Return();
        }
        /// <summary> 피격판정이 발생하는곳으로, 피해를 입고 피해경직 상태를 실행시킴</summary>
        public float Hit(float damage)
        {
            HP -= damage;
            _Behavior?.HitStop(0.15f);
            return 0;
        }
    }
}