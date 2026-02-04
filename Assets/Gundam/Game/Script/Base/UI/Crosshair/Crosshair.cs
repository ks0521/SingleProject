using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Sensor sensor;
    [SerializeField] private GameObject targetLockFrame;
    
    [SerializeField] private CrosshairSO _crosshairSo;

    private void Start()
    {
        if (sensor is not null)
        {
            sensor.OnTargetSensored += TargetLocked;
            sensor.OnTargetUnSensored += TargetUnLocked;
        }
    }

    private void TargetLocked(RaycastHit target)
    {
        targetLockFrame.SetActive(true);
        CrosshairExpend();
    }
    private void TargetUnLocked()
    {
        targetLockFrame.SetActive(false);
    }

    private void CrosshairExpend()
    {
        
    }
}
