using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class RepairContext : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI needCost;
    [SerializeField] private TextMeshProUGUI curMoney;
    [SerializeField] private TextMeshProUGUI curHp;
    [SerializeField] private TextMeshProUGUI shopRepair;
    private int _cost;
    private int _repairAmount;
    private int _hp => PlayerInfoManager.Instance.PlayerHp;
    private int _maxHp => PlayerInfoManager.Instance.PlayerMaxHp;
    private PlayerInfoManager playerInfo = PlayerInfoManager.Instance;
    private const int REPAIR_COST_CAP = 250;
    
    private void OnEnable()
    {
        SetRepairAmount();
        SetMoney();
        SetCost();
        SetHpContext();
    }

    public void SetMoney()
    {
        curMoney.text = playerInfo.Gold + "G";
    }
    /// <summary> 회복비용 계산, 상한 존재(250원)</summary>
    public void SetCost()
    {
        _cost = _repairAmount / 2;
        if (_cost > REPAIR_COST_CAP) _cost = REPAIR_COST_CAP;
        needCost.text = _cost + "G";
    }
    /// <summary> 플레이어의 체력과 최대체력 출력</summary>
    public void SetHpContext()
    {
        curHp.text = _hp + " / " + _maxHp;
    }
    /// <summary> 최대체력 70%회복, 최대체력보다 많이 회복되면 최대체력까지만 회복</summary>
    public void SetRepairAmount()
    {
        _repairAmount = (int)(_maxHp * 0.7);
        if (_repairAmount > _maxHp - _hp)
        {
            _repairAmount = _maxHp - _hp;
        }

        if (_repairAmount <= 0)
        {
            shopRepair.text = "회복됨";
            return;
        }
        shopRepair.text = _repairAmount.ToString();
    }

    public void TryRepair()
    {
        if (playerInfo.Gold < _cost)
        {
            Debug.Log("돈이 부족합니다");
            gameObject.GetComponent<Button>().interactable = false;
            return;
        }

        playerInfo.UseGold(_cost);
        playerInfo.SetHp(_hp+_repairAmount, _maxHp);
        SetRepairAmount();
        SetHpContext();
        SetCost();
        SetMoney();
        
    }
}