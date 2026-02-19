using System;
using System.Collections;
using System.Collections.Generic;
using Base.Managers;
using UnityEngine;

public class WorleMapEnterEffector : MonoBehaviour
{
    [SerializeField] private GameObject RewardPopUp;
    private void Start()
    {
        if (ScenesManager.Instance.canPopUpReward)
        {
            RewardPopUp.SetActive(true);
        }
    }
}
