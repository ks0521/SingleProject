using System;
using System.Collections;
using System.Collections.Generic;
using Contents.Player;
using Contents.Weapon;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

namespace Base.UI.WeaponSlot
{
    public class WeaponSlot : MonoBehaviour
    {
        [SerializeField] private Vector2 startPos;
        [SerializeField] private PlayerWeaponManager weaponManager;
        private WeaponParts _curWeaponParts;
        private RectTransform rt;
        private int _order; //자신이 몇번째 슬롯인지
        private bool isActive;
        private bool prevReload;
        private bool curReload;
        [SerializeField] private Image fill;
        [SerializeField] private Image frame;
        [SerializeField] private Image weaponImg;
        [SerializeField] private TextMeshProUGUI number;
        #region Color
        [Header("Colors")] 
        /* 컬러 사용규칙(~Frame : 슬롯 프레임, ~Fill : 슬롯 내부, ~Weapon : 무기이미지)
         * 사용중인 무기
         * 장전중 : ReloadingFrame + ReloadingFill + ActiveReloadingWeapon
         * 장전 완료 : ActiveReloadedFrame + ActiveReloadedFill + Weapon
         * 미사용중인 무기
         * 장전중 : ReloadingFrame + ReloadingFill + Weapon
         * 장전 완료 : InActiveReloadedFrame + InActiveReloadedFill + Weapon
         */
        private Color32 ReloadingFrameColor = new Color32(255,0,0,240);
        private Color32 ReloadingFillColor = new Color32(255,0,0,55);
        private Color32 ActivedReloadedFrameColor = new Color32(70,160,255,240);
        private Color32 ActivedReloadedFillColor= new Color32(70,160,255,60);
        private Color32 InActiveReloadedFrameColor= new Color32(143,143,143,240);
        private Color32 InActiveReloadedFillColor= new Color32(255,255,255,50);
        private Color32 ActiveReloadingWeaponColor= new Color32(255,0,0,240);
        private Color32 WeaponColor= new Color32(255,255,255,200);

        private Color32 ReloadingFill;
        private Color32 ReloadingFrame;
        private Color32 ReloadingWeapon;

        private Color32 ReloadedFill;
        private Color32 ReloadedFrame;
        private Color32 ReloadedWeapon;
        #endregion
        private void Awake()
        {
        }
        // MVP 구현 후 리팩토링(mvp / 이벤트 허브 등.. ) 방식 생각해 볼 것
        private void OnEnable()
        {
            startPos = GetComponent<RectTransform>().anchoredPosition;
            rt = GetComponent<RectTransform>();
            _order = transform.GetSiblingIndex();
            number.text = Convert.ToString(_order+1);
            
            weaponManager = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponManager>();
            if (weaponManager.weaponPartsList.Count < _order
                || weaponManager.weaponPartsList[_order] == null)
            {
                gameObject.SetActive(false);
            }
            weaponManager.OnChangeWeaponPart += ChangeWeapon;
            SetColor(isReloaded:true); //시작시에는 모든 무기가 장전된 채로 시작
            SetColorTemplet(false);
        }

        private void Start()
        {
            _curWeaponParts = weaponManager.weaponPartsList[_order];
            weaponImg.sprite = _curWeaponParts.weaponImg;
            
            //아래 3가지 색상은 무기 사용/미사용여부 관계없이 공통
            ReloadingFrame = ReloadingFrameColor; 
            ReloadingFill = ReloadingFillColor;
            ReloadedWeapon = WeaponColor;
        }

        private void FixedUpdate()
        {
            if (_curWeaponParts is null)
            {
                fill.fillAmount = 1;
                return;
            }
            
            curReload = _curWeaponParts.CanShot;
            if (curReload != prevReload)
            {
                if (curReload) //이번 프레임에서 장전됨
                {
                    SetColor(isReloaded: true);
                }
                else //이번 프레임에서 발사됨
                {
                    SetColor(isReloaded: false);
                }
            }
            SetFill();
            prevReload = curReload;
        }

        /// <summary> 플레이어가 입력한 무기 번호를 이벤트로 받아서 자신이면 활성화, 여기서 parts는 안씀</summary>
        public void ChangeWeapon(WeaponParts part, int order)
        {
            if (order == _order)
            {
                if (isActive) return; //이미 사용중인 무기에 활성화신호 보내면 작업하지 않음
                _curWeaponParts = part;
                ActiveSlot();
            }
            else if (isActive)
            {
                InActiveSlot();
            }
            
            SetColorTemplet(isActive);
        }

        private void ActiveSlot()
        {
            Debug.Log("움직임");
            rt.DOAnchorPosY(startPos.y + 50, 0.1f);
            isActive = true;
        }
        private void InActiveSlot()
        {
            rt.DOAnchorPosY(startPos.y, 0.1f);
            isActive = false;
        }
        void SetFill() => fill.fillAmount = _curWeaponParts.CoolDownRatio;
        /// <summary> 장전 여부에 따른 무기 슬롯의 색상을 변경</summary>
        void SetColor(bool isReloaded)
        {
            if (isReloaded) //장전완료된 상태
            {
                fill.color = ReloadedFill;
                frame.color = ReloadedFrame;
                weaponImg.color = ReloadedWeapon;
            }
            else
            {
                fill.color = ReloadingFillColor;
                frame.color = ReloadingFrameColor;
                weaponImg.color = ReloadingWeapon;
            }
        }
        /// <summary> 현재 무기가 사용/미사용인지 확인해서 이에맞는 색배합 지정</summary>
        /// <param name="isActived">현재 무기 사용여부</param>
        void SetColorTemplet(bool isActived)
        {
            if (isActived) //사용중인 무기
            {
                ReloadingWeapon = ActiveReloadingWeaponColor;
                ReloadedFrame = ActivedReloadedFrameColor;
                ReloadedFill = ActivedReloadedFrameColor;
            }
            else //미사용중인 무기
            {
                ReloadingWeapon = WeaponColor;
                ReloadedFrame = InActiveReloadedFrameColor;
                ReloadedFill = InActiveReloadedFrameColor;
            }
            SetColor(curReload);
        }

        
    }
}
