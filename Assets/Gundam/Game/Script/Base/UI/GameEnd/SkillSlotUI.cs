using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    public void Set(string text)
    {
        if (label != null) label.text = text;
        gameObject.SetActive(true);
    }

    public void Clear(bool disable = false)
    {
        if (label != null) label.text = "";
        if (disable) gameObject.SetActive(false);
    }
}
