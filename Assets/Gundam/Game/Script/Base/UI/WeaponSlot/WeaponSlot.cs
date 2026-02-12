using System;
using System.Collections;
using System.Collections.Generic;
using Contents.Player;
using Contents.Weapon;
using UnityEngine;
using DG.Tweening;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField] private Vector2 startPos;
    [SerializeField] private PlayerWeaponManager weaponManager;
    private RectTransform rt;
    private int _order; //자신이 몇번째 슬롯인지
    private bool isActive;
    private bool isReady;
    [Header("Colors")]
    private Color CooldownColor;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        startPos = GetComponent<RectTransform>().anchoredPosition;
        rt = GetComponent<RectTransform>();
        _order = transform.GetSiblingIndex();
        weaponManager.OnChangeWeaponPart += ChangeWeapon;
    }

    void Start()
    {
    }
    /// <summary> 플레이어가 입력한 무기 번호를 이벤트로 받아서 자신이면 활성화</summary>
    /// <param name="order"></param>
    public void ChangeWeapon(WeaponParts part, int order)
    {
        if (order == _order)
        {
            if (isActive) return; //이미 사용중인 무기에 활성화신호 보내면 작업하지 않음
            ActiveSlot();
        }
        else if (isActive)
        {   
            InActiveSlot();
        }
    }

    private void ActiveSlot()
    {
        Debug.Log("움직임");
        rt.DOAnchorPosY(startPos.y+50,0.1f);
        isActive = true;
    }

    private void InActiveSlot()
    {
        rt.DOAnchorPosY(startPos.y, 0.1f);
        isActive = false;
    }
}
