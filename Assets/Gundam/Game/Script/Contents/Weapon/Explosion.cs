using System;
using System.Threading;
using Base.PoolSO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Contents.Weapon
{
    public enum ExplosionType
    {
        Decorate, //이펙트용
        Damaging //피해용
    }

    public class Explosion : MonoBehaviour
    {
        private PooledObject _pooledObject;
        [SerializeField] private ExplosionType explosion;
        private void Awake()
        {
            _pooledObject = GetComponentInParent<PooledObject>();
        }

        private void OnEnable()
        {
            if(explosion == ExplosionType.Decorate)
                Return(this.GetCancellationTokenOnDestroy()).Forget();
            else
            {
                //범위피해
            }
        }

        async UniTaskVoid Return(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.8f), cancellationToken: token);
            _pooledObject.Return();
        }
    }
}