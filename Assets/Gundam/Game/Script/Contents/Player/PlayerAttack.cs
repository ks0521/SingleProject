using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct MechStatus
{
    public int increseDmg;
    public int multipleDmg;
    public int increseFireRate;
    public int multipleFireRate;
    public GameLayer mechTeam;
}
namespace Contents.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private WeaponParts usingWeapon; //현재 사용중인 무기의 파츠
        private MechStatus stat;
        void Start()
        {
            stat = new MechStatus();
            stat.mechTeam = GameLayer.Ally;
        }
    
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                usingWeapon.Attack(stat);
            }
        }
    }

}
