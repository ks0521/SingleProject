using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SO.Player
{
    /// <summary> 타격성공시 피드백 받기위한 인터페이스</summary>
    public interface IAttackFeedbackReceiver
    {
        public void AttackSuccess(int damage);
    }
    public class PlayerAttackFeedback : MonoBehaviour, IAttackFeedbackReceiver
    {
        public event Action<int> PlayerAttackSuccess;
        public void AttackSuccess(int damage)
        {
            Debug.Log($"플레이어 {damage}피해 입힘");
            PlayerAttackSuccess?.Invoke(damage);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Debug.Log("히트마커 테스트");
                AttackSuccess(1);
            }
        }
    }
}