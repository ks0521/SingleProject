using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IHittable
{
    void OnHit(int damage);
}
public class Monster : MonoBehaviour, IHittable
{
    public int hp = 100;
    public bool isDied = false;
    public Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnHit(int damage)
    {
        Debug.Log("Monster Hit! Damage: " + damage);
        hp-= damage;
        if (hp <= 0)
        {
            anim.SetTrigger("isDead");
        }
    }
    
    public void MonsterDie()
    {
        Destroy(gameObject);
    }
}



