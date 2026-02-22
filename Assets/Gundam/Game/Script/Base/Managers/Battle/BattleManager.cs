using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Managers;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private void OnEnable()
    {
        DelayCall().Forget();
    }

    async UniTaskVoid DelayCall()
    {
        await UniTask.NextFrame();
        MonsterSpawner.Instance.OnAllEnemiesBroken += BattleClear;
    }
    private void OnDisable()
    {
        MonsterSpawner.Instance.OnAllEnemiesBroken -= BattleClear;
    }

    void BattleClear()
    {
        Debug.Log("Call BattleManager");
        ScenesManager.Instance.canPopUpReward = true; //전투 보상창 띄울수 있다는 플래그만 변경
        ScenesManager.Instance.LoadScene((int)Scenes.WorldMap,LoadType.BattleClear);
    }
}
