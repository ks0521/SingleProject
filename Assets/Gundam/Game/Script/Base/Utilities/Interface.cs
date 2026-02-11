using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Utilities
{
    public interface IHittable
    {
        /// <summary>
        /// 피해를 입고 입은 피해를 반환
        /// </summary>
        /// <returns>최종 피해량</returns>
        public float Hit(float damage);
    }
}

