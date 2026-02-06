using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Base.Manager.Test
{
    [System.Serializable]
    public struct PoolData
    {
        public int count;
        /*Enum타입이 아니라 SO로 쓴 이유는 총알, 몬스터 등 많은 오브젝트 풀 요소들을 관리하기 쉽게 하기 위해서
         * (Enum으로 하니까 하나의 Enum이 너무 길어짐, 스크립트로 관리하는게 좋을것 같음)*/
        public PoolID dataSO;
    }

    /// <summary> PoolData 한 종을 오브젝트 풀에 추가 / 사용 / 반환하는 클래스</summary>
    public class ObjectPool
    {
        PoolData _data;
        private GameObject _parent;
        [SerializeField]Queue<GameObject> _pool;

        /// <summary>한 종류의 풀 생성</summary>
        /// <param name="data">오브젝트 풀에 넣을 데이터</param>
        /// <param name="parent">부모로 설정할 오브젝트</param>
        public ObjectPool(PoolData data, GameObject parent)
        {
            _parent = parent;
            _data = data;
            _pool = new Queue<GameObject>();
            AddPool(_data.count);
        }

        /// <summary> 오브젝트 추가하고 부모(오브젝트풀) 설정 </summary>
        /// <param name="count">추가할 갯수</param>
        void AddPool(int count)
        {
            int i = 0;
            do
            {
                GameObject newObj = GameObject.Instantiate(_data.dataSO.poolingObj, _parent.transform);
                //풀링된 오브젝트가 ReturnPool시 알아야할 key값을 넣어줌
                if (newObj.TryGetComponent(out IObjectPooled obj))
                {
                    obj.SetReturnPoolKey(_data.dataSO);
                } 
                newObj.SetActive(false);
                _pool.Enqueue(newObj);
            } while (++i < count);
        }

        /// <summary> 풀에서 오브젝트 꺼내 사용</summary>
        /// <param name="pos">꺼낼때의 위치</param>
        /// <param name="rot">꺼낼때의 회전</param>
        /// <returns>사용할 수 있는 오브젝트</returns>
        public GameObject UsePool(Vector3 pos, Quaternion rot)
        {
            if (_pool.Count <= 0)
            {
                AddPool(_data.count / 3);
                _data.count += (_data.count / 3);
                Debug.Log($"풀 추가 : 현재 {_data.dataSO}의 최대 개수 : {_data.count}");
            }

            GameObject usingObj = _pool.Dequeue();
            usingObj.transform.position = pos;
            usingObj.transform.rotation = rot;
            usingObj.SetActive(true);
            return usingObj;
        }

        /// <summary> 사용이 끝난 오브젝트를 풀에 반환하는 함수</summary>
        /// <param name="returnedObj">반환할 오브젝트</param>
        public void ReturnPool(GameObject returnedObj)
        {
            returnedObj.SetActive(false);
            _pool.Enqueue(returnedObj);
        }
    }

    /// <summary>
    /// 오브젝트 풀을 만드는 클래스(실제 사용은 poolDic만 사용)
    /// </summary>
    public class ObjectPoolGenericManager : MonoBehaviour
    {
        [SerializeField] List<PoolData> data;

        //딕셔너리의 키를 SO로 저장, 사용할 때는 해당부분에서 SO변수 추가해서 키로 사용
        public static Dictionary<PoolID, ObjectPool> poolDic;

        private void Awake()
        {
            poolDic = new Dictionary<PoolID, ObjectPool>();
            for (int i = 0; i < data.Count; i++)
            {
                AddDictionary(data[i]);
            }

            Debug.Log("풀 생성 완료");
        }

        void AddDictionary(PoolData addData)
        {
            //키는 PoolData의 SO로 , 값은 PoolData로 지정하고 부모를 자기자신(오브젝트 풀)로 설정
            poolDic.Add(addData.dataSO, new ObjectPool(addData, parent: gameObject));
        }
    }
}