using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Contents.Mech;
using Contents.Weapon;
using Contnts.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Base.Utilities
{
    /// <summary> 플레이어와 npc기체 모두 사용하는 스크립트로 , 외부에서 값을 입력받아
    /// 실제 실행만 담당함</summary>
    public class MechBehavior : MonoBehaviour
    {
        private Rigidbody _rb;
        private Animator _animator;
        [SerializeField] private AnimationClip hitClip;
        private int _hitHash;
        private bool _canControl;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
            _canControl = true;
        }
        /// <summary> 플레이어와 NPC 공용, 기체 이동 </summary>
        /// <param name="axisX">X축 이동여부</param>
        /// <param name="axisZ">Z축 이동여부</param>
        /// <param name="speed">이동 속도</param>
        public void Move(in float axisX,in float axisZ,in float speed)
        {
            if (!_canControl) return;
            _rb.CustomMove(axisX,axisZ,speed);
        }
        /// <summary> NPC 기체만 이용, 기체를 회전시킴</summary>
        public void Rotate(Transform targetPos)
        {
            if (!_canControl) return;
        }
        
        /// <summary> 플레이어와 NPC 공용, 장착중인 무기 공격을 실행</summary>
        public void Attack(WeaponParts part, AimData aimData, MechStatus machStat)
        {
            if (!_canControl) return;
            part.Attack(aimData,machStat);
        }
        public void HitStop(float duration)
        {
            HitStop(this.GetCancellationTokenOnDestroy(), duration).Forget();
        }
        async UniTaskVoid HitStop(CancellationToken token,float duration )
        {
            _canControl = false;
            _rb.CustomMove(0,0,0);
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
            _canControl = true;
        }
    }
}