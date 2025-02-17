using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(GenerateGUID))]

public class SceneItemManger : SingletonMonobehaviour<SceneItemManger>, ISaveable
{
    // parentItem 和 itemPrefab 暂时没什么用？
    private Transform parentItem;
    [SerializeField] private GameObject itemPrefab = null;


    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get  { return _iSaveableUniqueID; }  set { _iSaveableUniqueID = value; } }


    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable() //启用的时候要调用 ISaveable_Register 完成 注册， 同时将 AfterSceneLoad 订阅给 AfterSceneloadEvent
    {
        ISaveable_Register();
        EventHandler.AfterSceneloadEvent += AfterSceneLoad;
    }

    private void OnDisable() // 取消 register 同时 取消订阅
    {
        ISaveable_Deregister();
        EventHandler.AfterSceneloadEvent -= AfterSceneLoad;
    }




    public void ISaveable_Deregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveable_Register()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this); // 将当前的manager 注册到 SaveLoadManager 的 iSaveableObjectList 列表中
    }

    //核心方法， 将场景中的物体全部存储下来
    public void ISaveable_StoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName);

        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] itemsInScene = FindObjectsOfType<Item>();

        foreach (Item item in itemsInScene)
        {
            //把场景上的所有item 转为 SceneItem ，并将其存储到 sceneItemList中
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            sceneItemList.Add(sceneItem);
        }

        SceneSave sceneSave = new SceneSave();
        sceneSave.listSceneItem =  sceneItemList; // 把上面转换得到的sceneItemList 加入 listSceneItemDictionary 中，用 “sceneItemList” 作为键 ， 表示当前场景中需要保存的是sceneItemList
        GameObjectSave.sceneData.Add(sceneName, sceneSave); // 把当前的sceneSave 保存到字典中， 键是 当前场景的名字
    }

    public void ISaveable_RestoreScene(string sceneName) //恢复场景数据
    {
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave)) // 根据场景名字， 取出对应场景中存放的所有数据 
        {
            if (sceneSave.listSceneItem != null ) // 看这个SceneSave 中是否有存放sceneItemList ， 如果有就取出对应的List<SceneItem>
            {
                //清楚当前场景上已有的item （就是做 场景加载的系统 之前， 每次载入新场景，场景中原始item 就会从新加载一次）
                DestroySceneItems();

                //将sceneItemList 存储的当前场景上面 “应该” 拥有 的item 全部实例化
                InstantiateSceneItems(sceneSave.listSceneItem);
            }

        }
    }



    public void InstantiateSceneItem(int itemCode , Vector3 itemPosition)
    {
        //生成单个物品的方法
        GameObject itemGameObject = Instantiate(itemPrefab , itemPosition , Quaternion.identity , parentItem);
        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }

    private void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {
        GameObject itemGameObject;

        foreach(SceneItem sceneItem in sceneItemList)
        {
            itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentItem);

            //重新把 SceneItem 转换到 item ，这里 itemGameObject 其实是一个 空物体，只挂有 Item ，而下面的代码给这个item 组件设置了名字和 itemcode 后， 这个item 就能显示出对应的sprite 了 
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
        }
    }

    private void DestroySceneItems()
    {
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>(); 

        for(int i =  itemsInScene.Length - 1; i > -1; i--)
        {
            Destroy(itemsInScene[i].gameObject);
        }
    }

    public GameObjectSave ISaveableSave()
    {
        ISaveable_StoreScene(SceneManager.GetActiveScene().name);
        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;
            ISaveable_RestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
