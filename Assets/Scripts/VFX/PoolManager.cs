using System;
using System.Collections.Generic;
using UnityEngine;

//�����
public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    private Dictionary<int , Queue<GameObject>> poolDictionary = new Dictionary<int , Queue<GameObject>>(); //ʵ��װ���˶��� �� pool 
    [SerializeField] private Pool[] pool = null; //�ļ� ���� ����ʵ�ʶ���� 
    [SerializeField] private Transform objectPoolTransform = null; //���ǹ���������ű��� PoolManager �Լ���transform 

    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
    }

    private void Start()
    {
        for(int i = 0; i < pool.Length; i++) //ͨ�� Pool [] ���� poolDictionary
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
                GameObject newObject = Instantiate(prefab , parentGameObject.transform) as GameObject;  //ʵ�ʴ�������
                newObject.SetActive(false);

                poolDictionary[poolKey].Enqueue( newObject );
            }
        }
    }

    /// <summary>
    /// ʵ�ʶ����ṩ�ķ��� 
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
