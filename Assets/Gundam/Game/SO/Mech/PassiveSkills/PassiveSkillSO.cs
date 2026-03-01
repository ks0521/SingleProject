using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum SkillType
{
    Attack,
    Defense,
    Utililty
}

public enum SkillRarity
{
    Common,
    Rare,
    Unique
}
[CreateAssetMenu(menuName = "Mech/Skills/PassiveSkill",order = 0)]
public class PassiveSkillSO : ScriptableObject
{
    public int id; // 스킬 고유 id
    public string skillName; //스킬 이름
    public SkillType type; // 스킬 종류(공격/방어/유틸리티)
    public SkillRarity rarity;
    [TextArea(2, 4)]public string advantageText; // 스킬 장점
    [TextArea(2, 4)]public string disAdvantageText; // 스킬 단점
    [TextArea(2, 4)]public string lore; //스킬 로어(설명)
    public BonusStat status; // 패시브 스킬 적용 스탯
    public bool isConditional; //조건부 스킬 여부
    public float activeHp; //스킬 활성화에 필요한 hp비율
}
