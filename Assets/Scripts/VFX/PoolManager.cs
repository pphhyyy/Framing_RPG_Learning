using System;
using System.Collections.Generic;
using UnityEngine;

//对象池
public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    private Dictionary<int , Queue<GameObject>> poolDictionary = new Dictionary<int , Queue<GameObject>>(); //实际装载了对象 的 pool 
    [SerializeField] private Pool[] pool = null; //文件 配置 并非实际对象池 
    [SerializeField] private Transform objectPoolTransform = null; //就是挂载了这个脚本的 PoolManager 自己的transform 

    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
    }

    private void Start()
    {
        for(int i = 0; i < pool.Length; i++) //通过 Pool [] 构建 poolDictionary
        {
            CreatePool(pool[i].prefab , pool[i].poolSize);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;

        GameObject parentGameObject = new GameObject(prefabName + "Anchor"); 

        parentGameObject.transform.SetParent(objectPoolTransform);

        if(!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());
            for(int i = 0; i < poolSize; i++)
            {
                GameObject newObject = Instantiate(prefab , parentGameObject.transform) as GameObject;  //实际创新物体
                newObject.SetActive(false);

                poolDictionary[poolKey].Enqueue( newObject );
            }
        }
    }

    /// <summary>
    /// 实际对外提供的方法 
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public GameObject ReuseObject(GameObject prefab , Vector3 position , Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID() ;
        if(poolDictionary.ContainsKey(poolKey))
        {
            GameObject objectToReuse = GetObjectFromPool(poolKey);

            ResetObject(position , rotation , objectToReuse , prefab ); 

            return objectToReuse;
        }

        else
        {
            Debug.Log("No object pool for " + prefab);
            return null;
        }
    }

    private void ResetObject(Vector3 position, Quaternion rotation, GameObject objectToReuse, GameObject prefab)
    {
        objectToReuse.transform.position = position;
        objectToReuse.transform.rotation = rotation;
        objectToReuse.transform.localScale = prefab.transform.localScale;
    }

    private GameObject GetObjectFromPool(int poolKey)
    {
        GameObject objectToReuse = poolDictionary[poolKey].Dequeue();

        poolDictionary[poolKey].Enqueue (objectToReuse);

        if (objectToReuse.activeSelf == true)
        {
            objectToReuse.SetActive(false);
        }

        return objectToReuse;
    }
}
