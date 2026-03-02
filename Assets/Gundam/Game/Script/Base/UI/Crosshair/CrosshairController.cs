using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using SO.Player;
using Contnts.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private HUDManager hudManager;
    [SerializeField] private PlayerAim sensor;
    [SerializeField] private PlayerAttackFeedback attackFeedback;
    [SerializeField] private GameObject targetLockFrame;
    [SerializeField] private GameObject hitMarker;
    [SerializeField] private CrosshairSO crosshairSo;
    private CancellationTokenSource _hitCts;

    private void Awake()
    {
        hudManager = GetComponentInParent<HUDManager>();
    }

    private void OnEnable()
    {
        hudManager.OnPlayerActived += Init;
    }

    void Init()
    {
        GameObject player = GameObject.FindWithTag("Player");
        sensor = player.GetComponent<PlayerAim>();
        attackFeedback = player.GetComponent<PlayerAttackFeedback>();
        sensor.OnTargetSensored += TargetLocked;
        sensor.OnTargetUnSensored += TargetUnLocked;
        attackFeedback.PlayerAttackSuccess += TargetHit;
    }

    private void TargetHit(int damage)
    {
        _hitCts?.Cancel();
        _hitCts?.Dispose();

        _hitCts = new CancellationTokenSource();
        ShowHitMarker(_hitCts.Token).Forget();
    }

    async UniTaskVoid ShowHitMarker(CancellationToken token)
    {
        hitMarker.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.3), cancellationToken: token);
        hitMarker.SetActive(false);
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

    private void OnDestroy()
    {
        _hitCts?.Cancel();
        _hitCts?.Dispose();
    }
    private void OnDisable()
    {
        sensor.OnTargetSensored -= TargetLocked;
        sensor.OnTargetUnSensored -= TargetUnLocked;
        attackFeedback.PlayerAttackSuccess -= TargetHit;
    }

}
