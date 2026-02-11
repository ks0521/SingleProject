using System;
using System.Collections;
using System.Collections.Generic;
using Base.Utilities;
using Contents.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Contents.Mech
{
    public class MechStat : MonoBehaviour, IHittable
    {
        public MechStatus stat;
        private PlayerController _controller;
        private float hp = 100;
        public float HP
        {
            get => hp;
            private set
            {
                hp = value;
                Debug.Log($"Now Hp {value}");
                if (hp < 0)
                {
                    Die();
                }
            }
        }

        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            stat = new MechStatus();
        }

        private void Start()
        {
            
        }

        public void Die()
        {
            Debug.Log($"{gameObject.name} Die");
        }
        /// <summary> 피격판정이 발생하는곳으로, 피해를 입고 피해경직 상태를 실행시킴</summary>
        public float Hit(float damage)
        {
            HP -= damage;
            _controller.HitStop();
            return 0;
        }
    }
}