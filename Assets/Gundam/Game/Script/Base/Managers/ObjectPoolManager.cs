using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Base.Manager
{
    
    [System.Serializable]
    public struct PoolData
    {
        public GameObject obj;
        public int count;
        public ObjectPoolType type;
    }

    /// <summary> PoolData 한 종을 관리하는 클래스</summary>
    public class ObjectPool
    {
        PoolData _data;
        private GameObject _parent;
        Queue<GameObject> _pool;

        public ObjectPool(PoolData data, GameObject obj)
        {
            _parent = obj;
            _data = data;
            _pool = new Queue<GameObject>();
            AddPool(_data.count);
        }
        /// <summary> 풀에 오브젝트 추가 </summary>
        /// <param name="count">추가할 갯수</param>
        void AddPool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject newObj = GameObject.Instantiate(_data.obj, _parent.transform);
                newObj.SetActive(false);
                _pool.Enqueue(newObj);
            }
        }

        public GameObject UsePool(Vector3 pos, Quaternion rot)
        {
            if (_pool.Count <= 0)
            {
                AddPool(_data.count / 3);
                Debug.Log($"풀 추가 : 현재 {_data.type}의 최대 개수 : {_data.count}");
            }

            GameObject usingObj = _pool.Dequeue();
            usingObj.transform.position = pos;
            usingObj.transform.rotation = rot;
            return usingObj;
        }

        public void ReturnPool(GameObject returnedObj)
        {
            returnedObj.SetActive(false);
            _pool.Enqueue(returnedObj);
        }
    }

    public class ObjectPoolManager : MonoBehaviour
    {
        public List<PoolData> data;
        public static Dictionary<ObjectPoolType, ObjectPool> poolDic;

        private void Awake()
        {
            poolDic = new Dictionary<ObjectPoolType, ObjectPool>();
            for (int i = 0; i < data.Count; i++)
            {
                AddDictionary(data[i]);
            }
            Debug.Log("풀 생성 완료");
        }

        void AddDictionary(PoolData data)
        {
            poolDic.Add(data.type, new ObjectPool(data, gameObject));
        }
    }
}