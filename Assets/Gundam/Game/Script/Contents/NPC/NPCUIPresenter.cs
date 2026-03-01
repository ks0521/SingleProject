using System.Collections;
using System.Collections.Generic;
using SO.Mech;
using UnityEngine;

public class NPCUIPresenter : MonoBehaviour
{
    [SerializeField]private MechHealth playerHealth;
    [SerializeField] private HPBar hpBar;
    void OnEnable()
    {
        playerHealth.OnHpChanged += HpChanged;
    }

    void HpChanged(int hp, int maxHp)
    {
        hpBar.UpdateUI(hp,maxHp);
    }
}
