using System;
using System.Collections.Generic;
using Base.Utilities;
using Contnts.Player;
using SO.Mech;
using SO.Weapon;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SO.NPC
{
    public enum State{Seek, Approach, Attack, Retreat, Reposition, Stunned}
    /// <summary> NPC의 전투 AI구현부</summary>
    public class NPCController : MonoBehaviour
    {
        private MechBehavior _behavior;
        private MechStatus _mechstatus;
        private MechWeaponInventory _weaponInventory;
        public AIParameter param;
        [SerializeField] private WeaponParts curWeapon;
        
        public Transform target; // mvp용
        public LayerMask obstacleMask ; 

        private State _state = State.Seek;

        private float _decisionTimer;
        private float _stateTimer;
        private float _strafeTimer;
        private int _strafeSign = 1; //1 : 오른쪽, -1 : 왼쪽
        //적이 가까이에 있으면 회전속도가 빠르고 멀리있으면 느려짐(플레이어 회피 구현용)
        [SerializeField] private float turnSpeedNearDeg = 390f; //타겟과 가까울 때 최대 회전속도
        [SerializeField] private float turnSpeedFarDeg = 110f; //타겟과 멀 때 최소 회전속도
        [SerializeField] private float turnNearDistance = 2f; //회전속도가 최대가 되는 최단거리
        [SerializeField] private float turnFarDistance = 12f; //회전속도가 최소가 되는 최장거리
        [SerializeField] private float attackTurnMultiflier = 1.0f;
        
        [SerializeField] private float _targetRefreshInterval = 0.5f;
        [SerializeField] private float _switchHysteresisRatio = 0.1f; 
        //타겟이 기존 타겟보다 10% 이상 가까울때 변경-> 바뀌는 횟수 안정화
        private float _targetRefreshTimer;

        private int _allyLayer;
        private int _enemyLayer;
        private Color lineColor;
        private void Awake() 
        {
            _behavior = GetComponent<MechBehavior>();
            _mechstatus = GetComponent<MechStatus>();
            _weaponInventory = GetComponent<MechWeaponInventory>();
            _weaponInventory.OnChangeWeaponPart += ChangeWeapon;
            _allyLayer = (int)GameLayer.Ally;
            _enemyLayer = (int)GameLayer.Enemy;

            //모든 오브젝트 동시 갱신 스파이크방지
            _targetRefreshTimer = Random.Range(0, _targetRefreshInterval); 
            
            obstacleMask = 1 << (int)GameLayer.Default;
        }

        private void OnEnable()
        {
            //풀링되었을 때는 따로 초기값 설정 안함
            if (_mechstatus.archeType == null) return; 
            param = _mechstatus.archeType.parameter;
            //현재는 첫번째 무기 고정 사용으로 무기변경은 onenable 한번만
        }

        void ChangeWeapon(WeaponParts weaponpart, int index)
        {
            curWeapon = weaponpart;
        }
        private void Update()
        {
            if (param == null) return;

            _decisionTimer -= Time.deltaTime;
            _stateTimer -= Time.deltaTime;
            RefreshTargetTick();
            Act();
            if (_decisionTimer <= 0f)
            {
                _decisionTimer = param.decisionInterval;
                ChangeTransition();
            }
        }

        void RefreshTargetTick()
        {
            if (MonsterSpawner.Instance == null) return;

            _targetRefreshTimer -= Time.deltaTime;
            if (_targetRefreshTimer > 0) return;
            _targetRefreshTimer = _targetRefreshInterval;

            if (!isValidTarget(target))
            {
                target = FindNearestTarget(forceSwitch: true);
                return;
            }

            Transform closerTarget = FindNearestTarget(forceSwitch: false);
            if (closerTarget != null) target = closerTarget;
        }

        private Transform FindNearestTarget(bool forceSwitch)
        {
            int myLayer = gameObject.layer;
            IReadOnlyList<GameObject> enemies = (myLayer == _allyLayer)
                ? MonsterSpawner.Instance.EnemyList
                : MonsterSpawner.Instance.AllyList;
            Vector3 myPos = transform.position;

            float closestDist = float.PositiveInfinity;
            Transform closestTarget = null;

            float currentDist = float.PositiveInfinity;
            if (isValidTarget(target))
            {
                currentDist = (target.position - myPos).sqrMagnitude;
            }

            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                if (!enemy.activeInHierarchy) continue;

                if (enemy.layer != ((myLayer == _allyLayer) ? _enemyLayer : _allyLayer)) continue;

                Vector3 targetDist = enemy.transform.position - myPos;
                targetDist.y = 0;
                float targetDistSqr = targetDist.sqrMagnitude;

                if (targetDistSqr < closestDist)
                {
                    closestDist = targetDistSqr;
                    closestTarget = enemy.transform;
                }
            }

            if (closestTarget == null) return null;
            if (forceSwitch) return closestTarget;

            float threshold = 1f - Mathf.Clamp01(_switchHysteresisRatio);
            if (closestDist < currentDist * threshold)
            {
                return closestTarget;
            }

            return target;
        }
        bool isValidTarget(Transform target)
        {
            if (target == null) return false;
            if (!target.gameObject.activeInHierarchy) return false;

            int enemyLayer = ((int)gameObject.layer == _allyLayer) ? _enemyLayer : _allyLayer;
            if (target.gameObject.layer != enemyLayer) return false;

            return true;
        }
        void ChangeTransition()
        {
            if (target == null)
            {
                ChangeState(State.Seek);
                return;
            }

            float dist = Vector3.Distance(transform.position, target.position);
            bool hasLos = HasLineOfSight(target);
            //피격중이면 멈춤
            //if(isStunned) {changeState(State.Stunned); return;}
            //적이 자신의 안전거리 안으로 들어오면 도주
            if (dist < param.minSafeRange)
            {
                //Debug.Log("State Change : Retreat");
                ChangeState(State.Retreat); return;
            }
            //적이 시야에 들어오지 않으면 재배치
            if (!hasLos)
            {
                //Debug.Log("State Change : Reposition");
                ChangeState(State.Reposition); return;
            }
            //적이 공격거리 바깥에 있으면 접근
            if (dist > param.attackRange)
            {
                //Debug.Log("State Change : Approach");
                ChangeState(State.Approach); return;
            }
            //공격거리 안이고 시야확보되면 공격
            //Debug.Log("State Change : Attack");
            ChangeState(State.Attack);
        }
        void Act()
        {
            switch (_state)
            {
                case State.Seek:
                    //Debug.Log("상태변화 : seek");
                    //타겟이 있으면 접근
                    if (target != null)
                    {
                        Move(TowardTargetDir());
                        UpdateLookTracking();
                    }
                    else
                    {
                        _behavior.ClearLookTarget();
                        StopMove();
                    }
                    break;
                case State.Approach:
                    //타겟에게 이동
                    Move(TowardTargetDir());
                    UpdateLookTracking();
                    break;
                case State.Retreat:
                    //타겟에게서 도주
                    Move(AwayFromTargetDir());
                    UpdateLookTracking();
                    break;
                case State.Reposition:
                    //옆으로 돌기
                    Move(GetStrafeOrbitDir());
                    UpdateLookTracking();
                break;
                case State.Attack:
                    TryFire();
                    Move(GetStrafeOrbitDir() * 0.2f);
                    UpdateLookTracking();
                    break;
                case State.Stunned:
                    StopMove();
                    break;
            }
        }

        Vector3 TowardTargetDir()
        {
            Vector3 dir = (target.position - transform.position);
            dir.y = 0f;
            return dir.normalized;
        }
        Vector3 AwayFromTargetDir()
        {
            Vector3 dir = (transform.position - target.position);
            dir.y = 0f;
            return dir.normalized;
        }

        Vector3 GetStrafeOrbitDir()
        {
            ChangeStrafeDirection(force : false);

            Vector3 toTarget = (target.position - transform.position);
            toTarget.y = 0;
            float dist = toTarget.magnitude;
            if (dist < 0.001f) return Vector3.zero;

            Vector3 toN = toTarget / dist;
            //좌우 수직 벡터
            Vector3 strafeDir = Vector3.Cross(Vector3.up,toN).normalized * _strafeSign;
            //거리 보정 : 목표보다 멀면 접근(+toN), 가까우면 이격(-toN)
            float centripetal = dist - param.desiredRange; // centripetal > 0 이면 현재 거리가 기체의 선호 거리보다 멂
                                                           // / < 0 이면 현재 거리가 기체의 선호 거리보다 가까움
            Vector3 rangeFix = toN * Mathf.Clamp(centripetal, -1f, 1f);
            Vector3 move = strafeDir * param.strafeWeight + rangeFix * param.rangeFixWeight;
            move.y = 0;
            return move.normalized;
        }
        /// <summary> 측면이동 방향을 변경(강제 or strafeTimer 시간마다)</summary>
        /// <param name="force"> 강제 변경 여부</param>
        void ChangeStrafeDirection(bool force)
        {
            _strafeTimer -= Time.deltaTime;
            //강제로 바꾸거나 타이머가 다되면 측면이동 방향 랜덤으로 변경
            if (force || _strafeTimer <= 0f)
            {
                _strafeTimer = param.strafeHoldTime;
                _strafeSign = (Random.value < 0.5) ? -1 : 1;
            }
        }
        bool HasLineOfSight(Transform target)
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 dest = target.position + Vector3.up * 0.5f;
            Vector3 dir = (dest - origin);
            float dist = dir.magnitude;
            if (dist < 0.001f) return true;
            lineColor = (gameObject.layer == (int)GameLayer.Ally) ? Color.blue : Color.red;
            Debug.DrawRay(origin, dir.normalized * dist, lineColor, 0.1f);
            

            if (Physics.Raycast(origin, dir.normalized, dist, obstacleMask))
            {
                return false;
            }

            return true;
        }
        void ChangeState(State next)
        {
            if (_state == next) return;
            _state = next;

            if (_state == State.Attack)
            {
                _stateTimer = param.attackBurstTime;
            }
        }
        
        void Move(Vector3 dir)
        {
            Vector3 localdir = transform.InverseTransformDirection(dir);
            localdir.y = 0;
            localdir.Normalize();
            _behavior.Move(localdir.x,localdir.z,_mechstatus.archeType.mechBaseStatus.walkSpeed);
        }
        
        void StopMove()
        {
            _behavior.Move(0,0,0);
        }

        void UpdateLookTracking()
        {
            if (target == null)
            {
                _behavior.ClearLookTarget();
                return;
            }

            float dist = Vector3.Distance(transform.position, target.position);

            float t = (turnNearDistance <= turnFarDistance) 
                ? 1f : Mathf.InverseLerp(turnNearDistance, turnFarDistance, dist);
            float turnDeg = Mathf.Lerp(turnSpeedNearDeg, turnSpeedFarDeg, t);
            _behavior.SetLookTarget(target,turnDeg * attackTurnMultiflier);
        }
        void TryFire()
        {
            if (!target.gameObject.activeInHierarchy || target == null) return;
            //Debug.Log("TryFire");
            _weaponInventory.Attack(GetAim());
            //_behavior.Attack(GetAim(),curWeapon,_mechstatus.runtimeBonusStat);
        }
        AimData GetAim()
        {
            if (target == null)
            {
                Debug.LogWarning("Target is Null");
                return default;
            }
            //target의 좌표가 y = 0이라서 몸통으로 조준하게 변경
            Vector3 _dir = (target.position + Vector3.up - curWeapon.FirePoint.position).normalized;
            return new AimData(_dir, target.position);
        }
    }
}