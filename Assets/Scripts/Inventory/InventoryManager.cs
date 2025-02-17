using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager> , ISaveable
{

    private UIInventoryBar InventoryBar;
    private static InventoryManager instance;
    private Dictionary<int, ItemDetails> itemDetailsDictionary;

    private int[] selectedInventoryItem;  // ��ѡ�еĴ���б������

    public List<InventoryItem> [] InventoryLists;

    [HideInInspector] public int[] inventoryListCapacityIntArray;

    [SerializeField] private SO_ItemList itemList = null;


    private string _iSaveableUniqueID;

    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set {  _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;

    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        CreatInventoryList();

        CreatItemDetailsDictionary();

        selectedInventoryItem = new int[(int)InventoryLocation.count];
        for(int i = 0; i < selectedInventoryItem.Length; i++)
        {
            selectedInventoryItem[i] = -1;  // awake ʱ��ʼ��������Ŀǰû��ѡ���κ�һ��InventoryItem 
        }

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        GameObjectSave = new GameObjectSave();
    }

    private void OnDisable()
    {
        ISaveable_Deregister();
    }

    private void OnEnable()
    {
        ISaveable_Register();
    }

    private void Start()
    {
        InventoryBar = FindObjectOfType<UIInventoryBar>();
    }

    private void CreatInventoryList()
    {
        InventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];
        //��InventoryLocation.count ��֪��Ҫ�������� InventoryList ������������ һ�� ��� һ�� ���ӣ�

        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            InventoryLists[i] = new List<InventoryItem>(); //��ÿһ��InventoryList ��� �����new List<InventoryItem>()
        }
        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];  //�����¼ÿһ�� InventoryList ������

        //�Ѵ���player �� InventoryList ��������Ϊ Settings �е� playerInitialInventoryCapacity 
        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
        
    }

    private void CreatItemDetailsDictionary() //����SO_Item �еõ���list ���� �ֵ䣬
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        foreach (ItemDetails item in itemList.ItemDetails)
        {
            itemDetailsDictionary.Add(item.itemCode, item); 
        }
    }

    public void AddItem(InventoryLocation inventoryLocation, Item item , GameObject gameObjectToDelete)
    {
        //�������Ǹ���һ�����ǣ���������ǰѳ����ϵ�������ӵ�Inventory �У���ɾ�������е�����
        AddItem(inventoryLocation, item);
        Destroy(gameObjectToDelete);

    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryLocation">Ҫ�Ž��ĸ�����������</param>
        /// <param name="item">�����item </param>
    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode; 
        List<InventoryItem> inventoryList = InventoryLists[(int) inventoryLocation ]; //��InventoryLists�еõ���Ҫ������Ǹ�inventoryList

        int itemPosition = FindItemInInventory(inventoryLocation, itemCode); // �鿴Ҫ����������� list �е�λ��

        if(itemPosition != -1) // �����Ϊ-1 ˵����ǰ��list �� �� ����Ҫ�����item 
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode); //û������Ҫ�����item 
        }

        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, InventoryLists[(int)inventoryLocation]);
    }

    public void AddItem(InventoryLocation inventoryLocation, int itemCode)
    {
        Debug.Log("AddItem: " + itemCode);
        List<InventoryItem> inventoryList = InventoryLists[(int)inventoryLocation];

        int itemPosition = FindItemInInventory(inventoryLocation,itemCode);

        if(itemPosition != -1)
        {
            Debug.Log("itemPosition" + itemPosition);
            AddItemAtPosition(inventoryList , itemCode , itemPosition);
        }
        else
        {
            Debug.Log("itemPosition" + itemPosition);
            AddItemAtPosition(inventoryList , itemCode);
        }

        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, InventoryLists[(int)inventoryLocation]);

    }


    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int itemPosition = -1)
    {

        InventoryItem _item = new InventoryItem();
        _item.itemCode = itemCode;
        if (itemPosition == -1)
        {
            _item.itemQuantity = 1;
            inventoryList.Add(_item);
        }
        else
        {
            Debug.Log("����"+ 
                _item.itemQuantity);
            _item.itemQuantity = inventoryList[itemPosition].itemQuantity + 1; // ��inventoryList�ҵ���Ӧ��item��itemQuantity ��һ��ֵ��������_item
            inventoryList[itemPosition] = _item; // �� _item ���� inventoryList �е����� 

        }

        //DebugPrintInventoryList(inventoryList);
    }

    //private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    //{
    //    foreach (InventoryItem item in inventoryList)
    //    {
    //        Debug.Log("Item Description:" + InventoryManager.Instance.GetiItemDetails(item.itemCode).itemDescription + "   Item Quantity:" + item.itemQuantity);
    //    }
    //    Debug.Log("***************************************************************");
    //}

    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = InventoryLists[(int)inventoryLocation];

        for(int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].itemCode == itemCode)
                return i;  // �����ǰlist ���ҵ��˺�����Ҫ����Ķ�����ͬ��item �ͷ������list ��Ÿ�item ��λ��
        }
        return -1;

    }

    public ItemDetails GetiItemDetails(int itemCode) // ͨ���ֵ佨����ֵ���õ�ItemDetails
    {
        ItemDetails itemDetails;

        if(itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else return null;
    }

    public ItemDetails GetItemDetails(int itemcode)
    {
        ItemDetails itemDetails;
        if(itemDetailsDictionary.TryGetValue(itemcode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }

    public string GetItemTypeDescription(ItemType type)
    {
        string ItemTypeDescription;
        switch (type)
        {
            case ItemType.Watering_tool:
                ItemTypeDescription = Settings.WateringTool;
                break;
       
            case ItemType.Hoeing_tool:
                ItemTypeDescription = Settings.HoeingTool;
                break;
            case ItemType.Chopping_tool:
                ItemTypeDescription = Settings.ChoppingTool;
                break;
            case ItemType.Breaking_tool:
                ItemTypeDescription = Settings.BreakingTool;
                break;
            case ItemType.Reaping_tool:
                ItemTypeDescription = Settings.ReapingTool;
                break;
            case ItemType.Collecting_tool:
                ItemTypeDescription = Settings.CollectingTool;
                break;
            default:
                ItemTypeDescription = type.ToString();
                break;

        }

        return ItemTypeDescription;
    }


    public void ISaveable_Register()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveable_Deregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }


    public GameObjectSave ISaveableSave()
    {
        // ����һ���µ� scene save 
        SceneSave sceneSave = new SceneSave();

        // ɾ����persistent������������г����� save ����
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        //��Ӵ������ �� persistent������save��
        sceneSave.listInvItemArray = InventoryLists;

        //��� ����������� �� persistent������save��
        sceneSave.intArrayDictionary = new Dictionary<string, int[]>();
        sceneSave.intArrayDictionary.Add("inventoryListCapacityIntArray", inventoryListCapacityIntArray);

        // �������Ϸ������ӳ���save
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;

    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID,out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // ��Ҫ���Ҵ���б�,���Ŷ�λ �����Ϸ����� savescene 
            if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene,out SceneSave sceneSave))
            {
                if(sceneSave.listInvItemArray != null)
                {
                    InventoryLists = sceneSave.listInvItemArray;

                    for(int i = 0; i < (int)InventoryLocation.count; i++)
                    {
                        EventHandler.CallInventoryUpdatedEvent((InventoryLocation)i, InventoryLists[i]);
                    }

                    Player.Instance.ClearCarriedItem();

                    InventoryBar.ClearHighlightOnInventorySlot();
                }

                if(sceneSave.intArrayDictionary != null && sceneSave.intArrayDictionary.TryGetValue("inventoryListCapacityArray",out int[] inventoryCapacityArray) )
                {
                    inventoryListCapacityIntArray = inventoryCapacityArray;
                }
            }
        }
    }


    public void ISaveable_StoreScene(string sceneName)
    {

    }

    public void ISaveable_RestoreScene(string sceneName)
    {

    }


    internal void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryItems = InventoryLists[(int)inventoryLocation];

        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryItems , itemCode , itemPosition );
        }

        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, InventoryLists[(int)inventoryLocation]);
    }

    private void RemoveItemAtPosition(List<InventoryItem> inventoryItems, int itemCode, int itemPosition)
    {
        InventoryItem item = new InventoryItem();

        int quantity = inventoryItems[itemPosition].itemQuantity - 1;

        if(quantity > 0)
        {
            item.itemQuantity = quantity; 
            item.itemCode = itemCode;
            inventoryItems[itemPosition] = item;
        }

        else
        {
            inventoryItems.RemoveAt(itemPosition);
        }
    }

    internal void SwapInventoryItems(InventoryLocation inventoryLocation, int from_slotNumber, int toSlotNumber)
    {
        if(from_slotNumber < InventoryLists[(int)inventoryLocation].Count && toSlotNumber < InventoryLists[(int)inventoryLocation].Count)
        {
            //Ҫ���϶�����ʼλ�ú�Ŀ��λ�õ�����ֵС�� ����Ʒ��������

            //ͨ����ʼλ�úͽ���λ�ã��ҵ�Ҫ��������������

            InventoryItem fromInventoryItem = InventoryLists[(int)inventoryLocation][from_slotNumber];
            InventoryItem toInventoryItem = InventoryLists[(int)inventoryLocation][toSlotNumber];

            //����
            InventoryLists[(int)inventoryLocation][from_slotNumber] = toInventoryItem;
            InventoryLists[(int)inventoryLocation][toSlotNumber] = fromInventoryItem;

            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, InventoryLists[(int)inventoryLocation]);


        }
    }

    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation ] = itemCode; //��ʾ��ǰ��list �� plyaer ���������� �� ѡ�е�ĳһ������ ����code ��ʾ��
    }

    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1; //��ʾ��ǰlist û��ѡ������
    }

    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
    }

    public ItemDetails GetSelectedInventoryItemDetails (InventoryLocation inventoryLocation)
    {
        
        int itemcode = GetSelectedInventoryItem(inventoryLocation);
        //Debug.Log("itemcode::" + itemcode);
        if (itemcode == -1) // ����Ǹ�һ��˵��û�����屻ѡ��
        {
            return null;
        }
        else
        {
            return GetItemDetails(itemcode);
        }
    }

    
}
