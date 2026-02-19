using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Base.Battle
{
    public class SpawnPoint : MonoBehaviour
    {
        public bool IsUsed { get; private set; }

        public bool IsReserved { get; private set; }

        public bool TryUse()
        {
            if (IsUsed || IsReserved) return false;
            IsReserved = true;
            return true;
        }

        public void Use() => IsUsed = true;
        public void Reserve() => IsReserved = true;
    }
}