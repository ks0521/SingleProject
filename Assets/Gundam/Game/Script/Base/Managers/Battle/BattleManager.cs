using System;
using Base.Managers;
using Cysharp.Threading.Tasks;
using SO.Mech;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public event Action OnTimerChecked;
    private float _curTime;
    private void OnEnable()
    {
        DelayCall().Forget();
    }

    private void Awake()
    {
        Instance = this;
        _curTime = 0;
        
    }

    private void FixedUpdate()
    {
        _curTime += Time.fixedDeltaTime;
        if (_curTime >= 1f)
        {
            OnTimerChecked?.Invoke(); //1초마다 타이머 이벤트 뿌림
            _curTime = 0;
        }
    }

    async UniTaskVoid DelayCall()
    {
        await UniTask.NextFrame();
        if (MonsterSpawner.Instance != null)
        {
            MonsterSpawner.Instance.OnAllEnemiesBroken += BattleClear;
        }
    }
    private void OnDisable()
    {
        if (MonsterSpawner.Instance != null)
        {
            MonsterSpawner.Instance.OnAllEnemiesBroken -= BattleClear;
        }
    }

    void BattleClear()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null || !player.TryGetComponent(out MechHealth health))
        {
            Debug.LogWarning("BattleClear failed: Player or MechHealth not found.");
            return;
        }

        Debug.Log("Call BattleManager");
        int repairhp = (int)(health.Hp * 1.7); //70% 회복
        if (repairhp > health.MaxHp) repairhp = health.MaxHp;
        PlayerInfoManager.Instance.SetHp(repairhp, health.MaxHp);
        Debug.Log($"전투 후 남은 체력 : {repairhp} / {health.MaxHp}");
        ScenesManager.Instance.canPopUpReward = true; //전투 보상창 띄울수 있다는 플래그만 변경
        ScenesManager.Instance.LoadScene((int)Scenes.WorldMap,LoadType.BattleClear);
    }
}
