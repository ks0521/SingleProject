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

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private WeaponParts _curWeaponParts;
        [SerializeField] private MeleeComboAttack meleeComboAttack;
        private MechBehavior _behavior;
        private MechStatus _stat;
        private MechAnimation _ani;
        private PlayerWeaponManager _weaponManager;
        private AttackInvoker _attackInvoker;
        private PlayerAim _playerAim;
        private AimData _curAim;
        #region MoveValue
        private Rigidbody _rb;
        private float _axisX;
        private float _axisZ;
        private bool _canControl; //경직 여부
        private float _walkSpeed; //걷기 스피드
        private float _boosterSpeed; //부스터 스피드
        private float _speed; //현재 스피드
        private Vector3 _jumpVector;
        private AniMove _curMove;
        private AniMove _prevMove;
        #endregion
        //나중에 조건부 스킬 추가(대쉬시 / 점프시 .... )
        public event Action<AniMove> OnMoveStateChanged;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _playerAim = GetComponent<PlayerAim>();
            _attackInvoker = GetComponent<AttackInvoker>();
            _behavior = GetComponent<MechBehavior>();
            _weaponManager = GetComponent<PlayerWeaponManager>();
            _stat = GetComponent<MechStatus>();
            _ani = GetComponent<MechAnimation>();
            meleeComboAttack = GetComponentInChildren<MeleeComboAttack>();
        }

        private void OnEnable()
        {
            _weaponManager.OnChangeWeaponPart += ChangeWeapon;
        }

        void Start()
        {
            _speed = _stat._baseStatue.walkSpeed;
            _boosterSpeed = _stat._baseStatue.runSpeed;
            _curMove = AniMove.Idle;
            _prevMove = _curMove;
            _canControl = true;
            Debug.Log($"boosterSpeed init = {_boosterSpeed}");
        }

        void ChangeWeapon(WeaponParts weaponpart, int index)
        {
            Debug.Log($"{index} 번째 장비 장착 ");
            _curWeaponParts = weaponpart;
        }

        private void FixedUpdate()
        {
            if (!_canControl || meleeComboAttack.IsPlaying) return; //피격중이거나 공격중이면 이동안함
            _behavior.Move(_axisX, _axisZ, _speed);
        }
        
        void Update()
        {
            if (!_canControl) return;
            _axisX = Input.GetAxisRaw("Horizontal");
            _axisZ = Input.GetAxisRaw("Vertical");
            if (Input.GetMouseButton(0))
            {
                _curAim = _playerAim.GetAim();
                //현재 장착중인 무기부위와 조준 데이터를 가져옴
                _behavior.Attack(in _curAim,in _curWeaponParts,in _stat.RuntimeBonusStat);
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                //_ani.Jump(true);
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                _behavior.JetPackOn(2);
            }
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                //_ani.Jump(false);
            }
            
            //정지시 idle로 애니메이션 변경
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Debug.Log($"{_stat}{_stat._baseStatue}{_stat.RuntimeBonusStat}");
                //부스터 게이지 판정해서 바꾸기
                _speed = _stat._baseStatue.runSpeed+_stat.RuntimeBonusStat.increseSpeed;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _speed = _stat._baseStatue.walkSpeed+_stat.RuntimeBonusStat.increseSpeed;
            }
            //상태변화에 따른 이벤트 트리거 발생용(ex. 부스터 사용시 / 일정시간 정지 후 이동시)
            if (_curMove != _prevMove)
            {
                OnMoveStateChanged?.Invoke(_curMove);
                _prevMove = _curMove;
            }
        }
    }

}
