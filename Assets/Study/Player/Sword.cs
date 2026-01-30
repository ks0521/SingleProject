using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public HashSet<IHittable> hitset;
    public bool canAttack = false;
    IHittable hit;
    private void Start()
    {
        hitset = new HashSet<IHittable>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (!canAttack) return;
        hit = other.GetComponentInParent<IHittable>();
        if (hit != null && !hitset.Contains(hit))
        {
            hit.OnHit(25);
            hitset.Add(hit);
        }
    }
    public void AttackStart()
    {
        canAttack = true;
    }
    public void AttackEnd()
    {
        hitset.Clear();
        canAttack = false;
    }

}
