using System;
using System.Collections;
using System.Collections.Generic;
using Contnts.Player;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private PlayerAim sensor;
    [SerializeField] private GameObject targetLockFrame;
    [SerializeField] private CrosshairSO crosshairSo;

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
