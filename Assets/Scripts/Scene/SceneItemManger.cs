using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(GenerateGUID))]

public class SceneItemManger : SingletonMonobehaviour<SceneItemManger>, ISaveable
{
    // parentItem �� itemPrefab ��ʱûʲô�ã�
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

    private void OnEnable() //���õ�ʱ��Ҫ���� ISaveable_Register ��� ע�ᣬ ͬʱ�� AfterSceneLoad ���ĸ� AfterSceneloadEvent
    {
        ISaveable_Register();
        EventHandler.AfterSceneloadEvent += AfterSceneLoad;
    }

    private void OnDisable() // ȡ�� register ͬʱ ȡ������
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
        SaveLoadManager.Instance.iSaveableObjectList.Add(this); // ����ǰ��manager ע�ᵽ SaveLoadManager �� iSaveableObjectList �б���
    }

    //���ķ����� �������е�����ȫ���洢����
    public void ISaveable_StoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName);

        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] itemsInScene = FindObjectsOfType<Item>();

        foreach (Item item in itemsInScene)
        {
            //�ѳ����ϵ�����item תΪ SceneItem ��������洢�� sceneItemList��
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            sceneItemList.Add(sceneItem);
        }

        SceneSave sceneSave = new SceneSave();
        sceneSave.listSceneItem =  sceneItemList; // ������ת���õ���sceneItemList ���� listSceneItemDictionary �У��� ��sceneItemList�� ��Ϊ�� �� ��ʾ��ǰ��������Ҫ�������sceneItemList
        GameObjectSave.sceneData.Add(sceneName, sceneSave); // �ѵ�ǰ��sceneSave ���浽�ֵ��У� ���� ��ǰ����������
    }

    public void ISaveable_RestoreScene(string sceneName) //�ָ���������
    {
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave)) // ���ݳ������֣� ȡ����Ӧ�����д�ŵ��������� 
        {
            if (sceneSave.listSceneItem != null ) // �����SceneSave ���Ƿ��д��sceneItemList �� ����о�ȡ����Ӧ��List<SceneItem>
            {
                //�����ǰ���������е�item �������� �������ص�ϵͳ ֮ǰ�� ÿ�������³�����������ԭʼitem �ͻ���¼���һ�Σ�
                DestroySceneItems();

                //��sceneItemList �洢�ĵ�ǰ�������� ��Ӧ�á� ӵ�� ��item ȫ��ʵ����
                InstantiateSceneItems(sceneSave.listSceneItem);
            }

        }
    }



    public void InstantiateSceneItem(int itemCode , Vector3 itemPosition)
    {
        //���ɵ�����Ʒ�ķ���
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

            //���°� SceneItem ת���� item ������ itemGameObject ��ʵ��һ�� �����壬ֻ���� Item ��������Ĵ�������item ������������ֺ� itemcode �� ���item ������ʾ����Ӧ��sprite �� 
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
