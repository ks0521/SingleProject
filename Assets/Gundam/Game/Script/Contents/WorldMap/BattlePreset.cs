using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePreset : MonoBehaviour
{
    [SerializeField] private List<BattleSpawnPresetSO> battlePresets;
    [SerializeField] private BattleSpawnPresetSO curBattlePreset;
    private void OnEnable()
    {
        int rand = UnityEngine.Random.Range(0, battlePresets.Count);
        curBattlePreset = battlePresets[rand];
    }

    public void SetBattlePreSet()
    {
        StageManager.Instance.battlePreset = curBattlePreset;
    }
}
