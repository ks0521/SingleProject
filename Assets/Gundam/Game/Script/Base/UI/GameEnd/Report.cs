using System;
using System.Collections;
using System.Collections.Generic;
using Base.Managers;
using SO.Mech;
using TMPro;
using UnityEngine;

public class Report : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI result;
    [SerializeField] private TextMeshProUGUI type;

    private void OnEnable()
    {
        switch (PlayerInfoManager.Instance.ArcheTypeSo.rangeType)
        {
            case MechRangeType.Short:
                type.text = "강습형";
                break;
            case MechRangeType.Middle:
                type.text = "전술형";
                break;
            case MechRangeType.Long:
                type.text = "화력형";
                break;
        }

        if (ScenesManager.Instance._isVictory)
        {
            result.text = "작전 성공";
        }
        else
        {
            result.text = "작전 중 실종";
        }
    }
}
