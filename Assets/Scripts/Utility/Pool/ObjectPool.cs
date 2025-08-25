using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Services.AssetProvider;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Utility.Pool
{
    public abstract class ObjectPool<T> : IObjectPool<T> where T : Component, IPoolableObject
    {
        private readonly DiContainer container;
        private readonly IAssetsProvider assetsProvider;

        private readonly Dictionary<T, Queue<T>> pools = new();
        private readonly Dictionary<T, T> instanceToPrefab = new();

        protected Func<T, UniTask<T>> CreateObjectFunc;

        public ObjectPool(IAssetsProvider assetsProvider, DiContainer container)
        {
            this.assetsProvider = assetsProvider;
            this.container = container;

            CreateObjectFunc = CreateObject;
        }

        public async UniTask<T> Pop(string path)
        {
            T prefab = await assetsProvider.LoadPrefab<T>(path);
            return await Pop(prefab);
        }

        public async UniTask<T> Pop(T prefab)
        {
            if (!pools.TryGetValue(prefab, out var queue))
            {
                queue = new Queue<T>();
                pools[prefab] = queue;
            }

            if (queue.Count > 0)
            {
                T instance = queue.Dequeue();
                instance.gameObject.SetActive(true);

                instance.OnPop();
                return instance;
            }

            T newInstance = await CreateObjectFunc(prefab);
            instanceToPrefab[newInstance] = prefab;

            newInstance.OnPop();
            return newInstance;
        }

        public void Push(T instance)
        {
            instance.OnPush();
            instance.gameObject.SetActive(false);

            if (!instanceToPrefab.TryGetValue(instance, out var prefab))
            {
                Debug.LogWarning($"[GenericPrefabPool] Returned object not created by this pool: {instance.name}");
                Object.Destroy(instance.gameObject); // або ігнорувати

                return;
            }

            pools[prefab].Enqueue(instance);
        }

        public void Unload()
        {
            foreach (KeyValuePair<T, Queue<T>> pool in pools)
            {
                foreach (T pooledObject in pool.Value)
                {
                    if (pooledObject.gameObject)
                    {
                        Object.Destroy(pooledObject.gameObject);
                    }
                }
            }
            
            pools.Clear();
        }

        private UniTask<T> CreateObject(T prefab)
        {
            T newInstance = container.InstantiatePrefabForComponent<T>(prefab);
            return UniTask.FromResult(newInstance);
        }
    }
}