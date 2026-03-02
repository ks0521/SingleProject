using System;
using System.Collections;
using System.Collections.Generic;
using SO.Mech;
using UnityEngine;

public class UIPresenter : MonoBehaviour
{
    [SerializeField] private HUDManager hudManager;
    [SerializeField]private MechHealth playerHealth;
    [SerializeField] private MonsterSpawner spawner;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private ScoreUI allyScore;
    [SerializeField] private ScoreUI enemyScore;

    private void Awake()
    {
        hudManager = GetComponent<HUDManager>();
        hudManager.OnPlayerActived += Init;
    }

    void Init()
    {
        Debug.Log("HUD 초기화 시작");
        GameObject player = GameObject.FindWithTag("Player");
        playerHealth = player.GetComponent<MechHealth>();
        playerHealth.OnHpChanged += HpChanged;
        playerHealth.Refresh();
    }
    void OnEnable()
    {
        spawner.OnAllyNpcRemain += AllyScore;
        spawner.OnEnemyNpcRemain += EnemyScore;
    }

    void HpChanged(int hp, int maxHp)
    {
        hpBar.UpdateUI(hp,maxHp);
    }

    void AllyScore(int remain)
    {
        allyScore.ScoreRenew(remain.ToString());
    }

    void EnemyScore(int remain)
    {
        enemyScore.ScoreRenew(remain.ToString());
    }
}
