using System;
using System.Collections;
using System.Collections.Generic;
using SO.Mech;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TypeSelector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI typeName;
    [SerializeField] private TextMeshProUGUI typeContextHead;
    [SerializeField] private TextMeshProUGUI typeContext;
    [SerializeField] private TextMeshProUGUI weaponsHead;
    [SerializeField] private TextMeshProUGUI weapons;

    private MechTypeInfo _selectedTypeInfo;
    [SerializeField] private MechTypeInfo assultInfo;
    [SerializeField] private MechTypeInfo skermisherInfo;
    [SerializeField] private MechTypeInfo artilleryInfo;
    [SerializeField] private MechArcheTypeSO assult;
    [SerializeField] private MechArcheTypeSO skermisher;
    [SerializeField] private MechArcheTypeSO artillery;
    
    public void SelectAssult() { ContextChange(assultInfo); PlayerInfoManager.Instance.SelectType(assult);}
    public void SelectTactical() { ContextChange(skermisherInfo); PlayerInfoManager.Instance.SelectType(skermisher);}
    public void SelectFirePower() { ContextChange(artilleryInfo); PlayerInfoManager.Instance.SelectType(artillery);}

    private void OnEnable()
    {
        ContextChange(assultInfo); //창 활성화시 기본선택은 강습형
        PlayerInfoManager.Instance.SelectType(assult);
    }

    void ContextChange(MechTypeInfo typeInfo)
    {
        typeName.text = typeInfo.typeName;
        typeContext.text = typeInfo.typeContext;
        weaponsHead.text = typeInfo.weaponsHead;
        weapons.text = typeInfo.weapons;
    }
}
