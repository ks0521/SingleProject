using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using SO.Mech;
using SO.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MechAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private AnimationClip _hitClip;
    private PlayerController _controller;
    private MechHealth _health;
    private MechStatus _status;
    private Rigidbody _rb;
    
    private int _hitMultiPlier = Animator.StringToHash("HitMultiplier");
    private int _hitHash=  Animator.StringToHash("Hit");
    private int _speedHash =  Animator.StringToHash("Speed");
    private int _jumpHash= Animator.StringToHash("Jumping");
    private int _landingHash = Animator.StringToHash("Landing");
    private float _speed;
    private float _walkSpeed;
    private float _runSpeed;
    private float _speedThreshold;
    private Vector3 _speedVector;
    private Ray _ray;
    private bool _isJumping;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _health = GetComponent<MechHealth>();
        _status = GetComponent<MechStatus>();
    }

    void Start()
    {
        _health.OnHitStopped += HitStopAnimation;
    }

    void HitStopAnimation(float duration)
    {
        if (_status.SuperArmor) return; //경직면역이면 피격되어도 움직임
        _animator.SetFloat(_hitMultiPlier, _hitClip.length / duration);
        _animator.SetTrigger(_hitHash);
    }
    // Update is called once per frame
    void Update()
    {
        _speedVector = _rb.velocity;
        _speedVector.y = 0;
        _walkSpeed = _status.archeType.mechBaseStatus.walkSpeed + _status.runtimeBonusStat.increseSpeed;
        _runSpeed = _status.archeType.mechBaseStatus.runSpeed + _status.runtimeBonusStat.increseSpeed;
        _speed = _speedVector.magnitude;
        _ray = new Ray(transform.position, Vector3.down * 0.1f);
        
        if (_rb.velocity.y > 1.5 && !_isJumping)
        {
            Jump();
            return;
        }
        if (_isJumping && _rb.velocity.y < 1.5 && Physics.Raycast(_ray, 1f)) //점프중 바닥에 닿았을 때
        {
            Landing();
            return;
        }
        if (_speed <= _walkSpeed)
        {
            _speedThreshold = Mathf.InverseLerp(0f, _walkSpeed, _speed);
        }
        else
        {
            _speedThreshold = 1f + Mathf.InverseLerp(_walkSpeed, _runSpeed, _speed);
        }
        _animator.SetFloat(_speedHash,_speedThreshold);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(_ray.origin,_ray.origin + _ray.direction * 0.1f);
    }

    public void Jump()
    {
        _isJumping = true;
        _animator.SetBool(_jumpHash,true);
        _animator.SetBool(_landingHash,false);
    }

    void Landing()
    {
        _isJumping = false;
        _animator.SetBool(_jumpHash,false);
        _animator.SetBool(_landingHash,true);
    }
}
