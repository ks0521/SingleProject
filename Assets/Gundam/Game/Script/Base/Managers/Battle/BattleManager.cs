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
        _curTime += Time.deltaTime;
        if (_curTime >= 1f)
        {
            OnTimerChecked?.Invoke(); //1초마다 타이머 이벤트 뿌림
            _curTime = 0;
        }
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
        MechHealth health = GameObject.FindWithTag("Player").GetComponent<MechHealth>();
        Debug.Log("Call BattleManager");
        PlayerInfoManager.Instance.SetHp(health.Hp, health.MaxHp);
        Debug.Log($"전투 후 남은 체력 : {health.Hp} / {health.MaxHp}");
        ScenesManager.Instance.canPopUpReward = true; //전투 보상창 띄울수 있다는 플래그만 변경
        ScenesManager.Instance.LoadScene((int)Scenes.WorldMap,LoadType.BattleClear);
    }
}
