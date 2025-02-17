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

    private int[] selectedInventoryItem;  // 被选中的存货列表的索引

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
            selectedInventoryItem[i] = -1;  // awake 时初始化，代表目前没有选中任何一个InventoryItem 
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
        //从InventoryLocation.count 得知需要创建几个 InventoryList （这里是两个 一个 玩家 一个 箱子）

        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            InventoryLists[i] = new List<InventoryItem>(); //给每一个InventoryList 填充 具体的new List<InventoryItem>()
        }
        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];  //这里记录每一个 InventoryList 的容量

        //把代表player 的 InventoryList 容量设置为 Settings 中的 playerInitialInventoryCapacity 
        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
        
    }

    private void CreatItemDetailsDictionary() //根据SO_Item 中得到的list 建立 字典，
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        foreach (ItemDetails item in itemList.ItemDetails)
        {
            itemDetailsDictionary.Add(item.itemCode, item); 
        }
    }

    public void AddItem(InventoryLocation inventoryLocation, Item item , GameObject gameObjectToDelete)
    {
        //和下面那个不一样的是，这个函数是把场景上的物体添加到Inventory 中，并删除场景中的物体
        AddItem(inventoryLocation, item);
        Destroy(gameObjectToDelete);

    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryLocation">要放进哪个“容器”中</param>
        /// <param name="item">放入的item </param>
    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode; 
        List<InventoryItem> inventoryList = InventoryLists[(int) inventoryLocation ]; //从InventoryLists中得到需要加入的那个inventoryList

        int itemPosition = FindItemInInventory(inventoryLocation, itemCode); // 查看要加入的物体在 list 中的位置

        if(itemPosition != -1) // 如果不为-1 说明当前的list 中 有 我们要加入的item 
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode); //没有我们要加入的item 
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
            Debug.Log("数量"+ 
                _item.itemQuantity);
            _item.itemQuantity = inventoryList[itemPosition].itemQuantity + 1; // 从inventoryList找到对应的item的itemQuantity 加一后赋值给函数的_item
            inventoryList[itemPosition] = _item; // 用 _item 更新 inventoryList 中的内容 

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
                return i;  // 如果当前list 中找到了和我们要加入的对象相同的item 就返回这个list 存放该item 的位置
        }
        return -1;

    }

    public ItemDetails GetiItemDetails(int itemCode) // 通过字典建立的值，得到ItemDetails
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
        // 建立一个新的 scene save 
        SceneSave sceneSave = new SceneSave();

        // 删除除persistent场景以外的所有场景的 save 数据
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        //添加存货数组 到 persistent场景的save中
        sceneSave.listInvItemArray = InventoryLists;

        //添加 存货容器数组 到 persistent场景的save中
        sceneSave.intArrayDictionary = new Dictionary<string, int[]>();
        sceneSave.intArrayDictionary.Add("inventoryListCapacityIntArray", inventoryListCapacityIntArray);

        // 给这个游戏物体添加场景save
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;

    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID,out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // 需要查找存货列表,试着定位 这个游戏对象的 savescene 
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
            //要求拖动的起始位置和目标位置的索引值小于 该物品栏的容量

            //通过起始位置和结束位置，找到要交换的两个物体

            InventoryItem fromInventoryItem = InventoryLists[(int)inventoryLocation][from_slotNumber];
            InventoryItem toInventoryItem = InventoryLists[(int)inventoryLocation][toSlotNumber];

            //交换
            InventoryLists[(int)inventoryLocation][from_slotNumber] = toInventoryItem;
            InventoryLists[(int)inventoryLocation][toSlotNumber] = fromInventoryItem;

            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, InventoryLists[(int)inventoryLocation]);


        }
    }

    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation ] = itemCode; //表示当前的list （ plyaer 或者是箱子 ） 选中的某一个物体 （用code 表示）
    }

    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1; //表示当前list 没有选中物体
    }

    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
    }

    public ItemDetails GetSelectedInventoryItemDetails (InventoryLocation inventoryLocation)
    {
        
        int itemcode = GetSelectedInventoryItem(inventoryLocation);
        //Debug.Log("itemcode::" + itemcode);
        if (itemcode == -1) // 如果是负一。说明没有物体被选中
        {
            return null;
        }
        else
        {
            return GetItemDetails(itemcode);
        }
    }

    
}
