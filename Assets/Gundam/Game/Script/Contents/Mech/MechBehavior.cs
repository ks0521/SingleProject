using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using Contents.Mech;
using Contents.Weapon;
using Contnts.Player;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Base.Utilities
{
    /// <summary> 플레이어와 npc기체 모두 사용하는 스크립트로 , 외부에서 값을 입력받아
    /// 실제 실행만 담당함</summary>
    public class MechBehavior : MonoBehaviour
    {
        private Rigidbody _rb;
        private bool _canControl;
        private AttackInvoker _attackInvoker;
        private MechStatus _status;
        private Transform _target;
        private float _turnSpeedDegPerSec = 360f;
        private Vector3 _jumpVector;
        private bool superArmor;
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _attackInvoker = GetComponent<AttackInvoker>();
            _status = GetComponent<MechStatus>();
            _canControl = true;
        }

        private void OnEnable()
        {
            GetComponent<MechHealth>().OnHitStopped += HitStop;
        }

        public void SetLookTarget(Transform target, float turnSpeedDegPerSec)
        {
            _target = target;
            _turnSpeedDegPerSec = turnSpeedDegPerSec;
        }
    
        public void ClearLookTarget()
        {
            _target = null;
        }

        private void FixedUpdate()
        {
            if (!_canControl) return;
            if(_target == null) return;

            Vector3 to = _target.position - transform.position;
            to.y = 0f;

            if (to.sqrMagnitude < 0.001f) return; //타겟과의 거리가 가까울때 예외처리

            Quaternion targetRot = Quaternion.LookRotation(to.normalized, Vector3.up);
            Quaternion newRot =
                Quaternion.RotateTowards(_rb.rotation, targetRot,
                    _turnSpeedDegPerSec * Time.fixedDeltaTime); //회전속도를 미리 지정한 회전각 / 초로 제한
            _rb.MoveRotation(newRot);
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
        /// <summary> 점프중</summary>
        /// <param name="power"> 상승벡터</param>
        public void JetPackOn(float power)
        {
            _jumpVector = _rb.velocity;
            _jumpVector.y = 2;
            _rb.velocity = _jumpVector;
        }
        /// <summary> NPC 기체만 이용, 기체를 회전시킴</summary>
        public void Rotate(Transform targetPos)
        {
            if (!_canControl) return;
            _rb.DOLookAt(targetPos.position, 0.1f);
        }
        
        /// <summary> 플레이어와 NPC 공용, 장착중인 무기 공격을 실행</summary>
        public void Attack(in AimData aimData,in WeaponParts part,in BonusStat mechStat)
        {
            if (!_canControl) return;
            Debug.Log("Attack Invoke");
            _attackInvoker.AttackInvoke(in aimData, in part, in mechStat);
            //part.Attack(aimData,mechStat);
        }
        /// <summary> 피격 경직을 구현하는 메서드,
        /// MechHealth의 OnHitStop을 구독하고 있음</summary>
        /// <param name="duration">경직시간</param>
        public void HitStop(float duration)
        {
            if (_status.SuperArmor) return; //경직면역상태면 피격상관없이 작동
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