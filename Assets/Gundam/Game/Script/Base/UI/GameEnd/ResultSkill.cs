using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSkill : MonoBehaviour
{
    [SerializeField] private bool disableEmptySlots = false;

    private List<SkillSlotUI> _slots;

    private void Awake()
    {
        // 부모 아래 있는 슬롯들 자동 수집 (비활성 포함)
        _slots = new List<SkillSlotUI>(GetComponentsInChildren<SkillSlotUI>(true));
    }

    private void OnEnable()
    {
        var pim = PlayerInfoManager.Instance;
        if (pim == null)
        {
            Debug.LogWarning("PlayerInfoManager.Instance가 없습니다.");
            return;
        }

        var skills = pim.GainedSkills; // IReadOnlyList
        Debug.Log(_slots.Count);
        for (int i = 0; i < _slots.Count; i++)
        {
            if (i < skills.Count && skills[i] != null)
            {
                Debug.Log($"{skills[i].skillName} is in {i} index");
                // 최소 안전 버전: SO의 name 사용 (필드명이 뭔지 몰라도 동작)
                _slots[i].Set(skills[i].skillName);
            }
            else
            {
                Debug.Log($"{i} is null");
                _slots[i].Clear(disableEmptySlots);
            }
        }
    }
}
