using System.Collections;
using System.Collections.Generic;
using Contents.Mech;
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
        Debug.Log("HPChanged");
        hpBar.UpdateUI(hp,maxHp);
    }
}
