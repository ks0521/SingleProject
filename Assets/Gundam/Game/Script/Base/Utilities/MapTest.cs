using System;
using System.Collections;
using System.Collections.Generic;
using Base.Managers;
using UnityEngine;

public class MapTest : MonoBehaviour
{
    [SerializeField] private PassiveSkillManager skillManager;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))skillManager.MakeReward(Scenes.Battle);
    }
}
