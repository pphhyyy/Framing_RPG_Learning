using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private int _itemUseGridRadius = 0;
    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; }

    private ItemType _selectedItemType = 0;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    private bool _cursorIsEnable = false;
    public bool CursorIsEnable { get => _cursorIsEnable; set => _cursorIsEnable = value; }

    private void OnDisable()
    {
        EventHandler.AfterSceneloadEvent -= SceneLoaded;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneloadEvent += SceneLoaded;
    }

    private void SceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if(CursorIsEnable)
        {
            DisplayCursor();
        }
    }

    private Vector3Int DisplayCursor()
    {
        if(grid != null)
        {
            Vector3Int gridPosition = GetGridPositionForCursor();

            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            //Debug.Log("gridPosition: " + gridPosition + "====playerGridPosition: " +playerGridPosition) ;

            SetCursorValidity(gridPosition, playerGridPosition); // 设置光标的有效范围

            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        }

        else
        {
            return Vector3Int.zero;
        }
    }

 
    private void SetCursorValidity(Vector3Int gridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();

        if(Mathf.Abs(gridPosition.x - playerGridPosition.x) > ItemUseGridRadius || Mathf.Abs(gridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            //Debug.Log("距离过大");
            //当前光标位置和玩家位置差距大于 有效光标的半径，设置为无效
            SetCursorToInvalid(); // 将光标设置为无效
            return;
        }

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);
        if(itemDetails == null) // 如果当前没有选中item 说明无效 
        {
            //Debug.Log("没有选中");
            SetCursorToInvalid();
            return;
        }
        //从光标的位置，获取对应gridmap 上 该 grid 的详细属性
        GridPropertyDetails gridPropertyDetails = GridPropertIesManager.Instance.GetGridPropertyDetails(gridPosition.x, gridPosition.y);

        if (gridPropertyDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if(!IsCursorValidForSeed(gridPropertyDetails))
                    {

                        //Debug.Log("seed 无法放置于此");
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.Commodity:
                    if (!IsCursorValidForCommodity(gridPropertyDetails))
                    {
                        //Debug.Log("Commodity 无法放置于此");
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.Watering_tool: // 这里这样写， 意思就算 遇见 Watering_tool 和 Hoeing_tool 都执行 下面的代码
                case ItemType.Hoeing_tool:
                case ItemType.Chopping_tool:
                case ItemType.Breaking_tool:
                case ItemType.Reaping_tool:
                case ItemType.Collecting_tool:
                    if (!IsCursorValidForTool(gridPropertyDetails , itemDetails))
                    {
                        //Debug.Log("此处无法使用工具");
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.none:
                    break;
                case ItemType.count:
                    break;

                    default:
                    break;
            }
        }
        else
        {
            Debug.Log("该 grid 无效");
            SetCursorToInvalid();
            return;
        }
    }

    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        switch(itemDetails.itemType)
        {
            case ItemType.Hoeing_tool : // 如果当前用的是 锄头 
                if(gridPropertyDetails.isDiggable == true && gridPropertyDetails.daysSinceDug == -1) // 且当前grid 可以 挖 且没有被 挖过 
                {
                    #region 需要检查当前位置上的 item 是否是 reapable 的 , 比如如果当前位置上有草，那么玩家挖地的时候也会把草一同挖掉
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                    List<Item> itemList = new List<Item>();
                    HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSize, 0f); //获得当前 grid 上 全部的 item 并 装入 itemList

                    #endregion

                    bool foundReapable = false;
                    foreach(Item item in itemList)
                    {
                        // 检查 当前 grid 上 找到的 item 中 有没有 type 是  Reapable_scenary 的 
                        if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                        {
                            foundReapable = true;
                            break;
                        }

                        
                    }
                    //检查当前 grid 上有没有 crop 有 crop 就不能挖
                    CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

                    if(cropDetails != null) // 如果根据 seedItemCode 在 so_CropDetailsList 中找到了对应的 crop 
                    {
                        return false;
                    }


                    if (foundReapable) // 如果当前 grid 中 有 Reapable_scenary 的 item 说明 这个 grid 不能挖 返回 false
                        { return false; }
                    else { return true; }
                }

                else 
                    return false;

            case ItemType.Watering_tool: // 如果当前用的是 花洒 
                if (gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.daySinceWatered == -1) // 且当前grid 被 犁 过，且还没被浇水 
                {
                    return true;
                }

                else
                    return false;
            case ItemType.Chopping_tool:
            case ItemType.Collecting_tool:
            case ItemType.Breaking_tool:
                //检查当前的 crop（通过 gridPropertyDetails 得到） 能否 用 目前选中的 tool 来采集 ， 同时检查 这个 item（crop） 是否完全成熟了

                //当前 grid 上是否有种子种下
                if (gridPropertyDetails.seedItemCode != -1 )
                {
                    //通过 seedItemCode 获得当前 crop 的 detail 
                    CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

                    if(cropDetails != null) // 如果根据 seedItemCode 在 so_CropDetailsList 中找到了对应的 crop 
                    {
                        // 如果当前 grid 上种下的 crop 生长日期大于对应 crop 需要的最长生长日期(就是 growthDay 数组 的最后一个 )，说明这个 crop 已经成熟了
                        if (gridPropertyDetails.growthDays >= cropDetails.growthDay[cropDetails.growthDay.Length - 1 ]) 
                            
                        {
                            //这个 crop 能用工具采摘吗
                            if(cropDetails.CanUseToolToHarvestCrop(itemDetails.itemCode))
                            {
                                return true ;
                            }
                            else
                            { return false; }
                        }
                        else
                            return false;
                    }
                    
                }
                return false;
            default:
                return false;
                
        }
    }

   

    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem; // 如果当前的格子 canDropItem 为真 说明这个格子对 IsCursorValidForCommodity 有效 所以直接返回 canDropItem 即可
    }

    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    public void DisableCursor()
    {
        cursorImage.color = Color.clear;
        CursorIsEnable = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnable = true;
    }

    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }


    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(Player.Instance.transform.position);
    }

    public Vector3Int GetGridPositionForCursor()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        
        return grid.WorldToCell(worldPosition);
    }

    private Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition); //世界坐标到屏幕坐标
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }

    private Vector3 GetWorldPositionForCursor()
    {
        return grid.CellToWorld(GetGridPositionForCursor());
    }

}
