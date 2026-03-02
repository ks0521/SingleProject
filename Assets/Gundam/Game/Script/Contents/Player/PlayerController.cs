using UnityEngine;
using Base.Utilities;
using System;
using System.Threading;
using Contnts.Player;
using Cysharp.Threading.Tasks;
using SO.Mech;
using SO.Weapon;

namespace SO.Player
{
    /// <summary>플레이어의 직접적인 이동 및 조작 관여</summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private MeleeComboAttack meleeComboAttack;
        private MechBehavior _behavior;
        private MechStatus _stat;
        private AimData _curAim;
        #region MoveValue
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
            _behavior = GetComponent<MechBehavior>();
            _stat = GetComponent<MechStatus>();
            meleeComboAttack = GetComponentInChildren<MeleeComboAttack>();
            
        }
        void Start()
        {
            _speed = _stat.archeType.mechBaseStatus.walkSpeed;
            _boosterSpeed = _stat.archeType.mechBaseStatus.runSpeed;
            _curMove = AniMove.Idle;
            _prevMove = _curMove;
            Debug.Log($"boosterSpeed init = {_boosterSpeed}");
        }

        private void FixedUpdate()
        {
            if (meleeComboAttack.IsPlaying) return; //피격중이거나 공격중이면 이동안함
            _behavior.Move(_axisX, _axisZ, _speed);
        }
        
        void Update()
        {
            _axisX = Input.GetAxisRaw("Horizontal");
            _axisZ = Input.GetAxisRaw("Vertical");
            
            
            if (Input.GetKey(KeyCode.LeftControl))
            {
                _behavior.JetPackOn(2);
            }
            
            //정지시 idle로 애니메이션 변경
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Debug.Log($"{_stat}{_stat.archeType.mechBaseStatus}{_stat.runtimeBonusStat}");
                //부스터 게이지 판정해서 바꾸기
                _speed = _stat.archeType.mechBaseStatus.runSpeed+_stat.runtimeBonusStat.increaseSpeed;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _speed = _stat.archeType.mechBaseStatus.walkSpeed+_stat.runtimeBonusStat.increaseSpeed;
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
