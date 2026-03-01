using System;
using System.Collections;
using System.Collections.Generic;
using SO.Mech;
using TMPro;
using UnityEngine;

public class TypeSelector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI typeName;
    [SerializeField] private TextMeshProUGUI typeContextHead;
    [SerializeField] private TextMeshProUGUI typeContext;
    [SerializeField] private TextMeshProUGUI weaponsHead;
    [SerializeField] private TextMeshProUGUI weapons;

    private MechTypeInfo _selectedTypeInfo;
    [SerializeField] private MechTypeInfo assult;
    [SerializeField] private MechTypeInfo tactical;
    [SerializeField] private MechTypeInfo firePower;

    public void SelectAssult() { ContextChange(assult); PlayerInfoManager.Instance.SelectType(MechRangeType.Short);}
    public void SelectTactical() { ContextChange(tactical); PlayerInfoManager.Instance.SelectType(MechRangeType.Middle);}
    public void SelectFirePower() { ContextChange(firePower); PlayerInfoManager.Instance.SelectType(MechRangeType.Long);}

    private void OnEnable()
    {
        ContextChange(assult); //창 활성화시 기본선택은 강습형
        PlayerInfoManager.Instance.SelectType(MechRangeType.Short);
    }

    void ContextChange(MechTypeInfo typeInfo)
    {
        typeName.text = typeInfo.typeName;
        typeContext.text = typeInfo.typeContext;
        weaponsHead.text = typeInfo.weaponsHead;
        weapons.text = typeInfo.weapons;
    }
}
