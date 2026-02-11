using UnityEngine;
using Base.Utilities;
using System;
using System.Threading;
using Contents.Mech;
using Contents.Weapon;
using Contnts.Player;
using Cysharp.Threading.Tasks;

namespace Contents.Player
{
    public struct ShotInfo
    {
        public WeaponParts curWeaponParts;
        public AimData aimData;
        public MechRuntimeStatus MechRuntimeStat;
    }
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]private MechBehavior _behavior;
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationClip _hitClip;
        
        [SerializeField] private WeaponParts _curWeaponParts;
        [SerializeField] private ShotInfo _shotInfo;
        private AttackInvoker _attackInvoker;
        private MechStatus _mechStatus;
        private PlayerAim _playerAim;
        private AimData _curAim;
        #region MoveValue
        private Rigidbody _rb;
        private float _axisX;
        private float _axisZ;
        private bool _canControl; //경직 여부
        private float _hitStopDuration; //경직 시간
        private float _walkSpeed; //걷기 스피드
        private float _boosterSpeed; //부스터 스피드
        private float _speed; //현재 스피드
        private float _animationSpeed; //애니메이션에 적용될 스피드(0~1)
        private Vector3 _jumpVector;
        private AniMove _curMove;
        private AniMove _prevMove;
        private int _hitHash;
        private int _speedHash;
        #endregion
        //나중에 조건부 스킬 추가(대쉬시 / 점프시 .... )
        public event Action<AniMove> OnMoveStateChanged;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _rb = GetComponent<Rigidbody>();
            _hitHash =  Animator.StringToHash("Hit");
            _speedHash =  Animator.StringToHash("Speed");
            _playerAim = GetComponent<PlayerAim>();
            _attackInvoker = GetComponent<AttackInvoker>();
            _behavior = GetComponent<MechBehavior>();
            _mechStatus = GetComponent<MechStatus>();
        }

        void Start()
        {
            _walkSpeed = 7;
            _speed = _walkSpeed;
            _boosterSpeed = 5;
            _hitStopDuration = 0.12f;
            _curMove = AniMove.Idle;
            _prevMove = _curMove;
            _canControl = true;
            
            Debug.Log($"boosterSpeed init = {_boosterSpeed}");

        }

        #region Hit
        public void HitStop()
        {
            HitStop(this.GetCancellationTokenOnDestroy(), _hitStopDuration).Forget();
            Debug.Log("Hit");
        }

        public async UniTaskVoid HitStop(CancellationToken token,float duration )
        {
            _canControl = false;
            _animator.speed = _hitClip.length / duration;
            _animator.SetTrigger(_hitHash);
            _rb.velocity = Vector3.zero;
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
            _animator.speed = 1;
            _canControl = true;
        }
        #endregion

        #region Move
        private void FixedUpdate()
        {
            if (!_canControl) return;
            _behavior.Move(_axisX, _axisZ, _speed);
        }
        
        void Update()
        {
            if (!_canControl) return;
            _axisX = Input.GetAxisRaw("Horizontal");
            _axisZ = Input.GetAxisRaw("Vertical");
            if (Input.GetMouseButtonDown(0))
            {
                _curAim = _playerAim.GetAim();
                Debug.Log($"{_mechStatus.RuntimeStatus}");
                //현재 장착중인 무기부위와 조준 데이터를 가져옴
                _attackInvoker.AttackInvoke(in _curAim,in _curWeaponParts,in _mechStatus.RuntimeStatus);
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                _jumpVector = _rb.velocity;
                _jumpVector.y = 2;
                _rb.velocity = _jumpVector;
            }
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                _jumpVector = _rb.velocity;
                _jumpVector.y = 0;
                _rb.velocity = _jumpVector;
            }

            //정지시 idle로 애니메이션 변경
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                //부스터 게이지 판정해서 바꾸기
                _speed = _boosterSpeed;
                _curMove = AniMove.Booster;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _speed = _walkSpeed;
                _curMove = AniMove.Idle;
            }
            
            if (Mathf.Abs(_axisX) <= 0.5f && Mathf.Abs(_axisZ) <= 0.5f)  _curMove = AniMove.Idle;
            else if(_curMove != AniMove.Booster)  _curMove = AniMove.Walk;
            //이동상태  변화시 모션변경
            _animator.SetFloat(_speedHash, Mathf.Clamp((float)_curMove,0f,2f),0.1f,Time.deltaTime);
            
            //상태변화에 따른 이벤트 트리거 발생용(ex. 부스터 사용시 / 일정시간 정지 후 이동시)
            if (_curMove != _prevMove)
            {
                OnMoveStateChanged?.Invoke(_curMove);
                _prevMove = _curMove;
            }
        }
        #endregion
    }

}
