using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField]private Slider _hpBar;
    void Awake()
    {
        _hpBar = GetComponent<Slider>();
    }

    public void UpdateUI(int hp, int maxHp)
    {
       _hpBar.value =  Mathf.Clamp01((float)hp/maxHp);
    } 
        
}
