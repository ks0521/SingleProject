using System;
using Base.Manager.Test;
using Contents.Weapon;
using UnityEngine;

public class WeaponParts : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private PoolID weaponId;
    [SerializeField] private Transform firePoint;
    private int _hittableLayer;
    private int _enemyLayer;
    private Vector3 _targetPos;
    private Vector3 _dir;
    private RaycastHit _hit;
    private RaycastHit _prevHit;
    private RaycastHit _curHit;

    private Ray _ray1;
    private Ray _ray2;
    private bool isEnemy;
    private bool wasEnemy;
    public event Action<RaycastHit> OnTargetSensored;
    public event Action OnTargetUnSensored;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(LayerMask.GetMask("Enemy"));
        //조준시 플레이어 콜라이더 제외
        _hittableLayer = 1 << (int)GameLayer.Default | 1<<(int)GameLayer.Enemy;
        _enemyLayer = 1 << (int)GameLayer.Enemy;

    }

    private void Update()
    {
        SetProjectileDirect();
        UpdateCrosshair();
    }
    /// <summary> 투사체 목적지 설정 </summary>
    void SetProjectileDirect()
    {
        //크로스헤어(화면 중앙) 기준 직선으로 레이캐스팅 해서 총알 목표지점 탐색
        _ray1 = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Physics.Raycast(_ray1, out RaycastHit hit, 1000f, _hittableLayer);
        _targetPos = _ray1.origin + _ray1.direction * 1000f;
    }
    /// <summary> 실제 발사되는 위치에서 목표위치 사이 적군있으면 크로스헤어 이펙트 활성화, 사라지면 비활성화</summary>
    void UpdateCrosshair()
    {
        //실제 발사위치에서 목표상에 적이 존재하는지 확인, 탐지되면 크로스헤어 이펙트 활성화
        _dir = (_targetPos - firePoint.position).normalized;
        _ray2 = new Ray(firePoint.position, _targetPos);
        Physics.Raycast(_ray2, out _hit, 1000f, _hittableLayer);
        wasEnemy = isEnemy;
        //hit의 레이어가 Enemy면 isEnemy = true
        if (_hit.collider is not null &&
            _hit.collider.gameObject.layer == (int)GameLayer.Enemy)
        {
            isEnemy = true;
        }
        //hit가 없거나 레이어가 Enemy가 아니면 isEnemy = false
        else
        {
            isEnemy = false;
        }
        // 이전 프레임에선 적이 있었지만 현재 프레임에서 없어진 경우 크로스헤어 이펙트 비활성화
        if (!isEnemy && wasEnemy)
        {
            Debug.Log($"레이캐스트 벗어남 {_hit.collider}");
            OnTargetUnSensored?.Invoke();
        }
        // 이전 프레임에선 적이 없었지만 현재 프레임에서 생긴 경우 크로스헤어 이펙트 활성화
        else if (isEnemy && !wasEnemy)
        {
            Debug.Log($"레이캐스트 진입 {_hit.collider}");
            OnTargetSensored?.Invoke(_hit);
        }
    }

    public void Attack(MechStatus stat)
    {
        if (weaponData.AttackType == AttackType.Raycast) //레이캐스트
        {
            //RaycastAttack()
        }
        else //투사체
        {
            GameObject obj = ObjectPoolGenericManager.poolDic[weaponId].UsePool
                (firePoint.position, firePoint.rotation);
            if (obj.TryGetComponent<ProjectileAttack>(out var projectileBullet))
            {
                projectileBullet.Init(GetComponentInParent<Collider>(), _dir, stat);
            }
            else
            {
                Debug.LogError("풀에서 잘못된 오브젝트를 뽑아옴");
                ObjectPoolGenericManager.poolDic[weaponId].ReturnPool(obj);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_ray2.origin,_targetPos);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(_ray1.origin,_targetPos);
    }
}