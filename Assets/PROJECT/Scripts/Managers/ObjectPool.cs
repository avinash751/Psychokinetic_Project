using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    static ObjectPool Instance;
    Dictionary<GameObject, Queue<GameObject>> objectPool = new();
    Dictionary<GameObject, GameObject> objectToPrefabMap = new();
    Dictionary<GameObject, Transform> poolParents = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void InitializePool(GameObject prefab, int initialSize = 1)
    {
        if (!Instance.objectPool.ContainsKey(prefab))
        {
            Instance.objectPool[prefab] = new Queue<GameObject>();
            string parentName = prefab.name + " Parent";
            GameObject poolParent = new GameObject(parentName);
            poolParent.transform.parent = Instance.transform;
            Instance.poolParents[prefab] = poolParent.transform;

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab, poolParent.transform);
                obj.SetActive(false);
                Instance.objectPool[prefab].Enqueue(obj);
                Instance.objectToPrefabMap[obj] = prefab;
            }
        }
    }

    public static GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!Instance.objectPool.ContainsKey(prefab))
        {
            InitializePool(prefab);
        }

        if (Instance.objectPool[prefab].Count > 0)
        {
            GameObject obj = Instance.objectPool[prefab].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // Expand the pool to accomodate more requests
            GameObject obj = Instantiate(prefab, position, rotation, Instance.poolParents[prefab]);
            obj.SetActive(true);
            Instance.objectToPrefabMap[obj] = prefab;
            return obj;
        }
    }

    public static void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        if (Instance.objectToPrefabMap.TryGetValue(obj, out GameObject prefab))
        {
            obj.transform.SetParent(Instance.poolParents[prefab]);
            Instance.objectPool[prefab].Enqueue(obj);
        }
        else
        {
            Debug.LogError("Returned object does not belong to any prefab in the pool.");
        }
    }
}