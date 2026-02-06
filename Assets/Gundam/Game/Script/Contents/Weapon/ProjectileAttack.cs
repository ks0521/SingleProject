using System;
using Base.Manager.Test;
using UnityEngine;
using Cysharp.Threading.Tasks;

public interface IObjectPooled
{
    public void SetReturnPoolKey(PoolID id);
}

namespace Contents.Weapon
{
    /// <summary> 투사체의 이동을 담당하는 클래스</summary>
    public class ProjectileAttack : MonoBehaviour, IObjectPooled
    {
        private Rigidbody rb;
        [SerializeField] private WeaponData _weaponData;
        [SerializeField] private PoolID _returnPoolKey;
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        
        private void OnEnable()
        {
            if (_weaponData is null)
            {
                Debug.LogWarning("WeaPonData is Null");
            }
            Task().Forget();
        }

        public void SetReturnPoolKey(PoolID id)
        {
            _returnPoolKey = id;
        }

        async UniTaskVoid Task()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            ObjectPoolGenericManager.poolDic[_returnPoolKey].ReturnPool(gameObject);
        }
        void Init()
        {
            
        }
    
        private void OnCollisionEnter(Collision other)
        {
            throw new NotImplementedException();
        }
    }

}
