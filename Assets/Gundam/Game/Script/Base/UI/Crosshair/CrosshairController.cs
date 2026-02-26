using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Contents.Player;
using Contnts.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private PlayerAim sensor;
    [SerializeField] private PlayerAttackFeedback attackFeedback;
    [SerializeField] private GameObject targetLockFrame;
    [SerializeField] private GameObject hitMarker;
    [SerializeField] private CrosshairSO crosshairSo;
    private CancellationTokenSource _hitCts;
    private void Start()
    {
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
        /*try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.3), cancellationToken: token);

        }
        catch (OperationCanceledException)
        {
            return;
        }*/
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
