using System;
using System.Collections.Generic;
using UnityEngine;

public class TypeOfObjectPool : MonoBehaviour
{
    private static TypeOfObjectPool _instance;
    public static TypeOfObjectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<TypeOfObjectPool>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("TypeOfObjectPool");
                    _instance = go.AddComponent<TypeOfObjectPool>();
                }
            }
            return _instance;
        }
    }

    private Dictionary<Type, Queue<GameObject>> pool = new Dictionary<Type, Queue<GameObject>>();
    private Transform root;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        if (root == null)
        {
            root = new GameObject("PoolRoot").transform;
            root.SetParent(transform);
        }
    }

    public void CreatePool<T>(GameObject prefab, int size = 1) where T : Component, IPoolable
    {
        Type type = typeof(T);

        if (!pool.ContainsKey(type))
        {
            pool[type] = new Queue<GameObject>();
        }

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab, root);
            obj.SetActive(false);
            pool[type].Enqueue(obj);
        }
    }

    public void AddToPool<T>(T obj) where T : Component, IPoolable
    {
        Type type = typeof(T);

        if (!pool.ContainsKey(type))
        {
            pool[type] = new Queue<GameObject>();
        }

        obj.OnDespawn();
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(root);
        pool[type].Enqueue(obj.gameObject);
    }

    public T GetFromPool<T>(GameObject prefab) where T : Component, IPoolable
    {
        Type type = typeof(T);
        GameObject obj;

        if (pool.ContainsKey(type) && pool[type].Count > 0)
        {
            obj = pool[type].Dequeue();
        }
        else
        {
            obj = Instantiate(prefab, root);
        }

        obj.SetActive(true);
        T component = obj.GetComponent<T>();
        component.OnSpawn();

        return component;
    }

    public void ClearPool()
    {
        foreach (var queue in pool.Values)
        {
            while (queue.Count > 0)
            {
                Destroy(queue.Dequeue());
            }
        }
        pool.Clear();
    }
}