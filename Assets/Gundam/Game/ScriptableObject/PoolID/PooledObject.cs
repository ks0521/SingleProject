using System.Collections;
using System.Collections.Generic;
using Base.Manager.Test;
using UnityEngine;

namespace Base.PoolSO
{
    /// <summary> 오브젝트 풀에 들어가는 오브젝트의 반환을 돕는 스크립트</summary>
    public class PooledObject : MonoBehaviour
    {
        private PoolID _id;
        /// <summary> 오브젝트 풀에서 instantiate한 후 해당 오브젝트의 returnID설정할 때 사용</summary>
        public void SetId(PoolID id) => _id = id; 
        /// <summary> 저장된 returnID로 반환함 </summary>
        public void Return() => PoolManager.poolDic[_id].ReturnPool(gameObject);
    }
}