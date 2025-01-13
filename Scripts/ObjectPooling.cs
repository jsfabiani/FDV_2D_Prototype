using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooling : MonoBehaviour
{
    //public static ObjectPooling pool;
    public GameObject objectToPool;
    public int amountToPool = 6;
    private List<GameObject> pooledObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }

    public void DestroyPooledObject(GameObject obj)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (obj == pooledObjects[i])
            {
                pooledObjects.RemoveAt(i);
                Destroy(obj);
            }
        }
    }
    
}
