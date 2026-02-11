using System;
using System.Collections;
using System.Collections.Generic;
using Contents.Weapon;
using Unity.VisualScripting;
using UnityEngine;

namespace Contnts.Player
{
    public readonly struct AimData
    {
        public readonly Vector3 dir;
        public readonly Vector3 targetPos;

        public AimData(Vector3 inputDir, Vector3 inputTargetPos)
        {
            dir = inputDir;
            targetPos = inputTargetPos;
        }
    }
    public class PlayerAim : MonoBehaviour
    {
        public float Distance;
        [SerializeField] private WeaponParts weaponPart; //장착중인 무기 부위(필요없으면 삭제할 예정)
        [SerializeField] private Transform defaultPoint; //무기 미장착시 임시포인트(메인카메라)
        [SerializeField] private Transform firePoint; //무기 발사부위
        private int _hittableLayer; //적군(Enemy) + 오브젝트(Defulat) 레이어
        
        private Vector3 _targetPos;
        private Vector3 _dir;
        
        private RaycastHit _hit;
        private bool isEnemyAimed;
        private bool wasEnemyAimed;

        private Ray _ray1;
        private Ray _ray2;
        public event Action<RaycastHit> OnTargetSensored;
        public event Action OnTargetUnSensored;

        private void Awake()
        {
            _hittableLayer = 1 << (int)GameLayer.Default | 1 << (int)GameLayer.Enemy;
            defaultPoint = Camera.main.transform;
            firePoint = weaponPart?.FirePoint;
            if (firePoint == null)
            {
                firePoint = defaultPoint;
            }
        }

        private void Update()
        {
            SetAim();
            UpdateCrosshair();
        }

        //나중에 무기 바꿀때 WeaponParts로 이벤트 발행하면 해당 이벤트 받아서 무기교체
        private void UpdateWeapon(WeaponParts part)
        {
            //무기 변경시 firePoint 변경
            weaponPart = part;
            firePoint = part.FirePoint;
        }

        public AimData GetAim()
        {
            return new AimData(_dir,_targetPos);
        }

        void SetAim()
        {
            //크로스헤어(화면 중앙) 기준 직선으로 레이캐스팅 해서 총알 목표지점 탐색
            _ray1 = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            Physics.Raycast(_ray1, out _hit, 1000f, _hittableLayer);
            if (_hit.collider is null)
            {
                _targetPos = _ray1.origin + _ray1.direction * 1000f; //타겟의 거리
            }
            else
            {
                _targetPos = _hit.point;
            }
            _dir = (_targetPos - firePoint.position).normalized; //실제 총구에서 목표까지의 경로
        }

        /// <summary> 실제 발사되는 위치에서 목표위치 사이 적군있으면 크로스헤어 이펙트 활성화, 사라지면 비활성화</summary>
        void UpdateCrosshair()
        {
            //실제 발사위치에서 목표상에 적이 존재하는지 확인, 탐지되면 크로스헤어 이펙트 활성화
            _ray2 = new Ray(firePoint.position, _dir);
            Physics.Raycast(_ray2, out _hit, 1000f, _hittableLayer);
            wasEnemyAimed = isEnemyAimed;
            //hit의 레이어가 Enemy면 isEnemy = true
            if (_hit.collider is not null &&
                _hit.collider.gameObject.layer == (int)GameLayer.Enemy)
            {
                isEnemyAimed = true;
            }
            //hit가 없거나 레이어가 Enemy가 아니면 isEnemy = false
            else
            {
                isEnemyAimed = false;
            }

            // 이전 프레임에선 적이 있었지만 현재 프레임에서 없어진 경우 크로스헤어 이펙트 비활성화
            if (!isEnemyAimed && wasEnemyAimed)
            {
                Debug.Log($"레이캐스트 벗어남 {_hit.collider}");
                OnTargetUnSensored?.Invoke();
            }
            // 이전 프레임에선 적이 없었지만 현재 프레임에서 생긴 경우 크로스헤어 이펙트 활성화
            else if (isEnemyAimed && !wasEnemyAimed)
            {
                Debug.Log($"레이캐스트 진입 {_hit.collider}");
                OnTargetSensored?.Invoke(_hit);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_ray1.origin, _ray1.direction * 1000f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_ray2.origin, _ray2.direction * 1000f);
        }
    }
}