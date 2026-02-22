using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPresenter : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI rarity;
    [SerializeField] private TextMeshProUGUI advantage;
    [SerializeField] private TextMeshProUGUI disAdvantage;
    [SerializeField] private TextMeshProUGUI lore;
    [SerializeField] private PassiveSkillManager skillManager;
    [SerializeField] private PassiveSkillSO skillSO;
    [SerializeField] private Image frame;
    [SerializeField] private Image fill;

    private Color32 offensiveFill = new Color32(255, 20, 20, 170);
    private Color32 offensiveFrame = new Color32(255, 0, 0, 255);
    private Color32 defensiveFill = new Color32(20, 20, 255, 175);
    private Color32 defensiveFrame = new Color32(0, 0, 255, 255);
    private Color32 utilityFill = new Color32(20, 255, 20, 175);
    private Color32 utilityFrame = new Color32(0, 255, 0, 255);

    public void SetSkill(PassiveSkillSO skill)
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(()=>skillManager.ChoiceSkill(skill));
        btn.onClick.AddListener(skillManager.TurnOff);
        skillName.text = skill.name;
        rarity.text = "[" + skill.rarity.ToString() + "]";
        advantage.text = skill.advantageText;
        disAdvantage.text = skill.disAdvantageText;
        lore.text = skill.lore;
        if (skill.type == SkillType.Attack)
        {
            fill.color = offensiveFill;
            frame.color = offensiveFrame;
        }
        else if (skill.type == SkillType.Defense)
        {
            fill.color = defensiveFill;
            frame.color = defensiveFrame;
        }
        else
        {
            fill.color = utilityFill;
            frame.color = utilityFrame;
        }
    }
}
