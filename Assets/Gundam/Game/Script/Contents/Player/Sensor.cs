using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public bool IsDetected { get; private set; }
    private Ray _ray;
    private RaycastHit _hit;
    private RaycastHit _prevHit;
    private RaycastHit _curHit;
    private int layerMask;

    public event Action<RaycastHit> OnTargetSensored;
    public event Action OnTargetUnSensored;
    private void Start()
    {
        layerMask = LayerMask.GetMask("Enemy");
        _prevHit = default;
        _curHit = default;
    }
    private void Update()
    {
        _ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        _prevHit = _hit;
        Physics.Raycast(_ray, out _hit, 1000f, layerMask);
        
        // 지금 프레임에서 레이캐스트가 적을 처음으로 탐지하지 못함
        if (_hit.collider is null && _prevHit.collider is not null)
        {
            OnTargetUnSensored?.Invoke();
            Debug.Log("레이캐스트 벗어남");
        }
        // 지금 프레임에서 레이캐스트가 적을 처음으로 탐지함
        else if (_hit.collider is not null && _prevHit.collider is null)
        {
            OnTargetSensored?.Invoke(_hit);
            Debug.Log("레이캐스트 진입");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_ray.origin,_ray.direction * 1000f);
    }
}
