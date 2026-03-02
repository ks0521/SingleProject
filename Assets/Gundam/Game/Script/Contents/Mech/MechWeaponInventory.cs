using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base.Utilities;
using Contnts.Player;
using SO.Weapon;
using UnityEngine;

public class MechWeaponInventory : MonoBehaviour
{
    [SerializeField]private MechStatus _stat;
    [SerializeField]private List<WeaponData> candidateWeaponList; //기체가 사용할 수 있는 무기리스트
    [SerializeField] private List<WeaponParts> usableWeaponParts; //실제 기체가 사용하는 무기부위
    private MechBehavior _behavior;
    public WeaponParts curWeapons;
    public IReadOnlyList<WeaponParts> WeaponParts => usableWeaponParts;
    public int Count => usableWeaponParts.Count;
    private int weaponCounts;
    //바꾼 장비(Controller + WeaponSlot UI)와 몇번 장비로 바꿨는지(WeaponSlot UI)
    public event Action<WeaponParts, int> OnChangeWeaponPart;

    private void Awake()
    {
        _behavior = GetComponent<MechBehavior>();
    }

    public void Init(List<WeaponData> candidate)
    {
        _stat = GetComponent<MechStatus>();
        candidateWeaponList = candidate;
        if (_stat.archeType == null) return;
        candidateWeaponList = _stat.archeType.weaponLoadOut.weapons;
        LinkingWeaponParts();
    }
    
    /// <summary> 기체의 아키타입에 있는 사용가능 무기(WeaponData)를 실제 기체의 WeaponParts와 연결 </summary>
    void LinkingWeaponParts()
    {
        List<WeaponParts> allWeaponParts = GetComponentsInChildren<WeaponParts>().ToList();
        bool isFind = false;
        foreach (var weapon in candidateWeaponList)
        {
            foreach (var weaponPart in allWeaponParts)
            {
                if (weaponPart.WeaponData == weapon) 
                {
                    usableWeaponParts.Add(weaponPart);
                    isFind = true;
                    break;
                }
            }
            if (isFind)  Debug.Log($"{usableWeaponParts.LastOrDefault()}무기 파츠 부착");
            else Debug.LogWarning($"{weapon} 무기의 부착 파츠를 찾을 수 없습니다");
            isFind = false;
        }
        Equip(0); //첫번째 무기로 초기화 - MVP에서는 NPC는 해당 위치에서만 무기를 착용함(1번무기 고정)
        OnChangeWeaponPart?.Invoke(curWeapons,0);
    }

    public void Equip(int index)
    {
        if (index < 0 || index >= usableWeaponParts.Count)
        {
            Debug.LogWarning("Equip: 현재 장비파츠의 인덱스를 벗어난 요구입니다");
            return;
        }

        if (curWeapons == usableWeaponParts[index])
        {
            Debug.Log("Equip : 이미 장착중인 장비입니다");
            return;
        }
        curWeapons = usableWeaponParts[index];
        Debug.Log($"{index} 번째 장비 장착 {curWeapons.WeaponData}");
        OnChangeWeaponPart?.Invoke(curWeapons, index);
    }

    public WeaponParts Get(int index)
    {
        if (index < 0 || index >= usableWeaponParts.Count)
        {
            Debug.LogWarning("Get: 현재 장비파츠의 인덱스를 벗어난 요구입니다");
            return null;
        }
        return usableWeaponParts[index];
    }

    public void Attack(in AimData curAim)
    {
        _behavior.Attack(in curAim,in curWeapons,in _stat.runtimeBonusStat);
    }
    //WeaponSlot 초기값 지정용
    public void Refresh() {OnChangeWeaponPart?.Invoke(usableWeaponParts[0], 0 );}
    private void OnDisable()
    {
        candidateWeaponList = null;
        usableWeaponParts.Clear();
    }
}
