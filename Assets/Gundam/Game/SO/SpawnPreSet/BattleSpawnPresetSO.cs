using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RangeCount
{
    public int shortRange;
    public int midRange;
    public int longRange;

    public int Total => shortRange + midRange + longRange;
}

[CreateAssetMenu(menuName = "Spawn Preset", fileName = "BattleSpawnPreset_")]
public class BattleSpawnPresetSO : ScriptableObject
{
    [Header("표시용")]
    public string presetName;
    [TextArea] public string description;

    [Header("스폰 조합")]
    public RangeCount ally;
    public RangeCount enemy;

    
    public int TotalAlly => ally.Total;
    public int TotalEnemy => enemy.Total ;
}
