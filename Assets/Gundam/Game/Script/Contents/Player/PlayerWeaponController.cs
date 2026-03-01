using System;
using System.Collections;
using System.Collections.Generic;
using Base.Utilities;
using Contnts.Player;
using SO.Weapon;
using UnityEngine;

namespace SO.Player
{
    /// <summary> 플레이어의 무기 변경, 발사 관여</summary>
    public class PlayerWeaponController : MonoBehaviour
    {
        private WeaponParts _curWeaponPart;
        private MechWeaponInventory _weaponInventory;
        private PlayerAim _playerAim;

        private void Awake()
        {
            _weaponInventory = GetComponent<MechWeaponInventory>();
            _playerAim = GetComponent<PlayerAim>();
        }
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                _weaponInventory.Attack(_playerAim.GetAim());
                //현재 장착중인 무기부위와 조준 데이터를 가져옴
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _weaponInventory.Equip(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
               _weaponInventory.Equip(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _weaponInventory.Equip(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _weaponInventory.Equip(3);
            }
        }
    }
}