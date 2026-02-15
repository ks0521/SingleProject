using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Contents.Mech;
using Contents.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MechAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private AnimationClip _hitClip;
    private MechHealth _mechHealth;
    private MechStatus _mechStatus;
    private PlayerController _controller;
    private Rigidbody _rb;
    private int _hitMultiPlier;
    private int _hitHash;
    private int _speedHash;
    private int _jumpHash;
    private float _speed;
    private float _walkSpeed;
    private float _runSpeed;
    private float _speedThreshold;
    private Vector3 _speedVector;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mechHealth = GetComponent<MechHealth>();
        _mechStatus = GetComponent<MechStatus>();
        _hitHash =  Animator.StringToHash("Hit");
        _hitMultiPlier = Animator.StringToHash("HitMultiplier");
        _speedHash =  Animator.StringToHash("Speed");
        _jumpHash = Animator.StringToHash("Jumping");
    }

    void Start()
    {
        _mechHealth.OnHitStopped += HitStopAnimation;
    }

    void HitStopAnimation(float duration)
    {
        _animator.SetFloat(_hitMultiPlier, _hitClip.length / duration);
        _animator.SetTrigger(_hitHash);
    }
    // Update is called once per frame
    void Update()
    {
        _speedVector = _rb.velocity;
        _speedVector.y = 0;
        _walkSpeed = _mechStatus._baseStatue.walkSpeed + _mechStatus.RuntimeStatus.increseSpeed;
        _runSpeed = _mechStatus._baseStatue.runSpeed + _mechStatus.RuntimeStatus.increseSpeed;
        _speed = _speedVector.magnitude;
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

    public void Jump(bool start)
    {
        _animator.SetBool(_jumpHash,start);
    }
}
