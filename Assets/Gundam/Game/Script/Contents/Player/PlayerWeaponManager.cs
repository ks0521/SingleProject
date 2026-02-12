using System;
using System.Collections;
using System.Collections.Generic;
using Contents.Weapon;
using UnityEngine;

namespace Contents.Player
{
    public class PlayerWeaponManager : MonoBehaviour
    {
        [SerializeField] private List<WeaponParts> weaponPartsList;
        [SerializeField] private WeaponParts _curWeaponPart;
        //바꾼 장비(Controller + WeaponSlot UI)와 몇번 장비로 바꿨는지(WeaponSlot UI)
        public event Action<WeaponParts, int> OnChangeWeaponPart; 
        void Start()
        {
            _curWeaponPart = weaponPartsList[0];
            OnChangeWeaponPart?.Invoke(_curWeaponPart,0);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (weaponPartsList[0] is not null && _curWeaponPart != weaponPartsList[0])
                {
                    _curWeaponPart = weaponPartsList[0];
                    OnChangeWeaponPart?.Invoke(_curWeaponPart, 0);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (weaponPartsList[1] is not null && _curWeaponPart != weaponPartsList[1])
                {
                    _curWeaponPart = weaponPartsList[1];
                    OnChangeWeaponPart?.Invoke(_curWeaponPart, 1);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (weaponPartsList[2] is not null && _curWeaponPart != weaponPartsList[2])
                {
                    _curWeaponPart = weaponPartsList[2];
                    OnChangeWeaponPart?.Invoke(_curWeaponPart, 2);
                }
            }
        }
    }
}