using System.Collections;
using System.Collections.Generic;
using Contents.Mech;
using UnityEngine;

public class UIPresenter : MonoBehaviour
{
    [SerializeField]private MechHealth playerHealth;
    [SerializeField] private MonsterSpawner spawner;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private ScoreUI allyScore;
    [SerializeField] private ScoreUI enemyScore;
    void OnEnable()
    {
        playerHealth.OnHpChanged += HpChanged;
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
