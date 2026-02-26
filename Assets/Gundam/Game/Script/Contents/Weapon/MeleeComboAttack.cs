using System;
using System.Threading;
using Base.Utilities;
using Contents.Player;
using Contents.Weapon;
using Contnts.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MeleeComboAttack : MonoBehaviour
{
    [SerializeField] private FirePointMarker meleeAttackPoint;
    private Rigidbody _rb;
    private Animator _animator;
    private MechStatus _status;
    private PlayerAttackFeedback _attackFeedback;
    private PlayerWeaponManager _weaponManager;
    //공격범위
    [SerializeField] private Vector3 halfExtents = new Vector3(1.2f, 1.0f, 1.2f);
    //대쉬중 적 탐지 범위(도주 고려해서 범위 더 작게)
    [SerializeField] private Vector3 laungeExtends = new Vector3(1.0f, 0.7f, 0.9f);
    [SerializeField] private int targetMask; //공격 대상

    private static readonly int IsMelee = Animator.StringToHash("IsMelee"); //공격 진행여부(복귀 트랜지션)
    private static readonly int Melee1 = Animator.StringToHash("Melee1"); //1타(오른손 펀치)
    private static readonly int Melee2 = Animator.StringToHash("Melee2"); //2타(왼손 펀치)
    private static readonly int Melee3 = Animator.StringToHash("Melee3"); //3타(발구르기)

    [SerializeField] private int _comboIndex;
    private bool _isComboPlaying;
    public bool IsPlaying => _isComboPlaying;
    private bool _isDelay; //공격 시작 후딜레이
    private bool _queueNext;
    private FinalStat _stat;
    private Vector3 _center;
    private Quaternion _rot;
    private CancellationTokenSource dashCts;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponentInParent<Rigidbody>();
        _status = GetComponentInParent<MechStatus>();
        _weaponManager = GetComponentInParent<PlayerWeaponManager>();
        _attackFeedback = GetComponentInParent<PlayerAttackFeedback>();
        targetMask = (gameObject.layer == (int)GameLayer.Ally) 
            ? 1 << (int)GameLayer.Enemy : 1 << (int)GameLayer.Ally;
    }

    private void OnEnable()
    {
        if (_weaponManager is null) return;
        _weaponManager.OnChangeWeaponPart += ForceEnd; //무기 바뀌면 강제멈춤
    }
    /// <summary> 첫번째 공격 시작 </summary>
    /// <param name="stat"> 공격 시작시점 스탯</param>
    public void StartCombo(in FinalStat stat)
    {
        _stat = stat;
        if (_isComboPlaying || _isDelay) //이미 근접공격중이거나 후딜이면 시작안함
            return;
        if (!Input.GetMouseButtonDown(0)) return;
        _isComboPlaying = true;
        _comboIndex = 1;

        Debug.Log("StartCombo called");
        _animator.ResetTrigger(Melee1);
        _animator.ResetTrigger(Melee2);
        _animator.ResetTrigger(Melee3);
        _animator.SetBool(IsMelee, true);
        StartDash(); //1타 대쉬
    }

    public void Update()
    {
        if (!_isComboPlaying) return;

        if (Input.GetMouseButtonDown(0))
        {
            _queueNext = true;
            Debug.Log("QueueNext");
        }
    }
    /// <summary> Animator에 넣는 메서드로, 범위내 적 피해입힘 </summary>
    public void AnimEvent_Hit()
    {
        _rot = meleeAttackPoint.transform.rotation;
        _center = meleeAttackPoint.transform.position;

        var cols = Physics.OverlapBox(_center, halfExtents, _rot, targetMask, QueryTriggerInteraction.Ignore);
        foreach (var target in cols)
        {
            if (target.TryGetComponent<IHittable>(out var hitter))
            {
                hitter.Hit(_stat.Damage);
                _attackFeedback?.AttackSuccess((int)_stat.Damage);
            }
        }
    }

    // 현재 타가 끝날 때 호출 (1->2, 2->3 진행 결정)
    public void AnimEvent_AdvanceCombo()
    {
        if (!_queueNext) //공격모션 진행중 추가입력 들어오지 않았으면 연속공격 종료
        {
            EndCombo();
            return;
        }

        _queueNext = false;
        if (_comboIndex == 1) //1타일때 추가입력 : 2타로 이동
        {
            _comboIndex = 2;
            _animator.SetTrigger(Melee2);
        }
        else if (_comboIndex == 2) //2타일때 추가입력 : 3타로 이동
        {
            _comboIndex = 3;
            _animator.SetTrigger(Melee3);
        }
        else //3타일 때 근접공격 종료
        {
            EndCombo();
        }
    }
    // 콤보 완전 종료(3타 끝 or 중간 끊김)
    public void AnimEvent_EndCombo()
    {
        EndCombo();
    }
    /// <summary> 대쉬 강제종료 </summary>
    /// <summary> 첫 공격 시작시 대쉬</summary>
    void StartDash()
    {
        if (_comboIndex != 1) return; //첫번째 공격이 아닌경우 실행하지 않음
        dashCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        _status.SuperArmorStatus(true); //근접공격 간 (공격 시작 대쉬 ~ 3타) 경직받지 않음
        Dash(1, dashCts.Token).Forget();
    }
    /// <summary> 공격범위에 적이 들어오거나 일정시간동안 이동속도가 상승함</summary>
    /// <param name="maxSecond"> 최대 대쉬 시간</param>
    async UniTaskVoid Dash(float maxSecond, CancellationToken token)
    {
        float endTime = Time.time + maxSecond;
        try
        {
            while (Time.time < endTime)
            {
                token.ThrowIfCancellationRequested();
                _rb.CustomMove(0f, 1f, _status._baseStatue.runSpeed);
                _rot = meleeAttackPoint.transform.rotation;
                _center = meleeAttackPoint.transform.position;
                //공격범위에 적이 들어오면 대쉬 종료
                if (Physics.CheckBox(_center, laungeExtends, _rb.rotation, targetMask))
                {
                    break;
                }
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate,token);
            }
            _animator.SetTrigger(Melee1);
        }
        catch (OperationCanceledException)
        {
            
        }
        finally //끝나면 정지하고 1타 애니메이션 실행
        {
            _rb.CustomMove(0f, 0f, _status._baseStatue.walkSpeed);
        }
    }
    void ForceStopDash()
    {
        dashCts?.Cancel();
        dashCts?.Dispose();
        dashCts = null;
    }

    //무기 바뀌었을 때 강제로 진행중이던 행동 멈춤
    public void ForceEnd(WeaponParts parts, int index)
    {
        ForceStopDash();
        EndCombo();
    }


    private void EndCombo()
    {
        if (!_isComboPlaying) return; //이미 콤보가 끝난상태면 실행안함
        Debug.Log("EndCombo called, IsMelee=false");
        MeleeAttackDelay(this.GetCancellationTokenOnDestroy()).Forget();
        _comboIndex = 0;
        _animator.SetBool(IsMelee, false);
        _isComboPlaying = false;
        _status.SuperArmorStatus(false);
    }

    //근접공격 후딜레이
    async UniTaskVoid MeleeAttackDelay(CancellationToken token)
    {
        _isDelay = true;
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);
        _isDelay = false;
    }

    private void OnDrawGizmos()
    {
        Vector3 center = meleeAttackPoint.transform.position;
        Quaternion rot = meleeAttackPoint.transform.rotation;
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(center, rot, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);
        Gizmos.matrix = Matrix4x4.identity;
    }
}