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
        if (Input.GetKeyDown(KeyCode.F2))skillManager.MakeReward(Scenes.Elite);
        if (Input.GetKeyDown(KeyCode.F3))skillManager.MakeReward(Scenes.Boss);

    }
}
