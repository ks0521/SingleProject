using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Sword sword;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        sword = GetComponentInChildren<Sword>();
        anim = GetComponent<Animator>();
    }
    public void AttackStart()
    {
        sword.AttackStart();
    }
    public void AttackEnd()
    {
        sword.AttackEnd();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetTrigger("Slash");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            anim.SetFloat("Speed", 1);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            anim.SetFloat("Speed", 0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            anim.SetTrigger("Dance");
        }
    }
}
