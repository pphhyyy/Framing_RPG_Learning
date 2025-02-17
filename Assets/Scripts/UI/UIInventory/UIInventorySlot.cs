using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Camera mainCamera;
    private Canvas parentCanvas;
    private Transform parentItem;
    private GridCursor gridCursor;
    private Cursor cursor;


    public GameObject draggedItem;
    public Image inventorySlotHighLight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;
    [HideInInspector] public ItemDetails itemDetails;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;
    [HideInInspector] public bool isSelected = false; // 当前物品槽是否被选中

    [SerializeField] private int slotNumber = 0; //当前物品在物品栏众的位置

    private float offest_Y = 0;
    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneloadEvent -= SceneLoaded;
        EventHandler.RemoveSelectedItemFromInventoryEvent -= RemoveSelectedItemFromInventoryEvent;
        EventHandler.DropSelectedItemEvent -= DropSelectedItemAtMousePosition;
    }

    
    private void OnEnable()
    {
        EventHandler.AfterSceneloadEvent += SceneLoaded;
        EventHandler.RemoveSelectedItemFromInventoryEvent += RemoveSelectedItemFromInventoryEvent;
        EventHandler.DropSelectedItemEvent += DropSelectedItemAtMousePosition;
    }

    private void RemoveSelectedItemFromInventoryEvent()
    {
        if(itemDetails != null && isSelected)
        {
            int itemcode = itemDetails.itemCode;

            InventoryManager.Instance.RemoveItem(InventoryLocation.player , itemcode);
            if(InventoryManager.Instance.FindItemInInventory(InventoryLocation.player , itemcode) == -1)
            {
                ClearSelectedItem();
            }
        }
    }

    private void SceneLoaded()
    {
        //这里需要在成绩加载完成以后，才能再场景中寻找有对应标签的 物体 ,ItemsParentTransform 就是对应场景上的 item 对象，其内部有一大堆item 
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }


    private void Start()
    {
        mainCamera = Camera.main;
        gridCursor = FindObjectOfType<GridCursor>();
        cursor = FindObjectOfType<Cursor>();

    }

    private void ClearCursors()
    {
        gridCursor.DisableCursor();
        cursor.DisableCursor();
        gridCursor.SelectedItemType = ItemType.none;
        cursor.SelectedItemType = ItemType.none;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(itemDetails != null)
        {
            Player.Instance.DisablePlayerInputAndResetMovement();
            draggedItem = Instantiate(inventoryBar.InventoryBarDraggedItem , inventoryBar.transform);
            //Debug.Log(draggedItem.name);
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;

            //开始拖动一个物体时，可以在物品栏上高亮显示这个物体
            SetSelectedItem();
            
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
            //Debug.Log(draggedItem.transform.position);
            //offest_Y = draggedItem.gameObject.GetComponent<RectTransform>().rect.size.y * 2.2f;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(draggedItem != null)
        {
            Destroy(draggedItem);

            if(eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>()!= null)
            {
                //拖动到了 物品栏 上 
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>().slotNumber; //拖动的目的地在物品栏中的位置

                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

                DestroyInventoryTextBox();

                //结束拖动取消物品栏上的高亮显示
                ClearSelectedItem();
            }
            else
            {
                if(itemDetails.canBeDropped)
                {
                    DropSelectedItemAtMousePosition();
                }
            }
        }

        Player.Instance.EnablePlayerInput();
    }

    private void DropSelectedItemAtMousePosition()
    {
        if(itemDetails != null && isSelected)
        {

            //这里映入 cursorpositionisVaild 以后 就不用 单独判断是否 canDropItem 了
            // //检测此处是否可以放置物品
            // Vector3Int gridPosition = GridPropertIesManager.Instance.grid.WorldToCell(worldPosition);
            //// Debug.Log(gridPosition);
            // GridPropertyDetails gridPropertyDetails = GridPropertIesManager.Instance.GetGridPropertyDetails(gridPosition.x, gridPosition.y);


            if (gridCursor.CursorPositionIsValid) // 如果当前网格拥有（可放置物品） 的属性 ， 就可以进行放置
            {
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x , Input.mousePosition.y , -mainCamera.transform.position.z));
                GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(worldPosition.x,worldPosition.y - Settings.gridCellSize/2f , worldPosition.z), Quaternion.identity, parentItem);
                Item item = itemGameObject.GetComponent<Item>();
                item.ItemCode = itemDetails.itemCode;

                InventoryManager.Instance.RemoveItem(InventoryLocation.player , item.ItemCode);

                //如果把物品脱出以后，当前物品在物品栏中已经没有剩余，就可以清空其高亮的标志
                if(InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.ItemCode) == -1)
                {
                    ClearSelectedItem();
                }

            }

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

       
        if(itemQuantity != 0)
        {
            
            inventoryBar.inventoryTextBoxGameObject = Instantiate(inventoryTextBoxPrefab , transform.position , Quaternion.identity);
            inventoryBar.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            inventoryTextBox.SetTextboxText(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            if(inventoryBar.isInventoryBarPositionBottom)
            {
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x , transform.position.y  + 50f, transform.position.z);
            }
            else
            {
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();
    }

    private void DestroyInventoryTextBox()
    {
        if(inventoryBar.inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {

            if( isSelected )
            {
                ClearSelectedItem();
            }
            else
            {
                if(itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }
        }
    }

    private void SetSelectedItem()
    {
        inventoryBar.ClearHighlightOnInventorySlot();
        isSelected = true;
        inventoryBar.SetHighlightOnInventorySlot();


        gridCursor.ItemUseGridRadius = itemDetails.itemUseGridRadius;
        cursor.ItemUseRadius = itemDetails.itemUseRadius;

        Debug.Log("itemDetails.itemUseGridRadius" + itemDetails.itemUseGridRadius);

        if(itemDetails.itemUseGridRadius > 0 )
        {
            gridCursor.EnableCursor();
        }
        else
        {
            gridCursor.DisableCursor();
        }

        if (itemDetails.itemUseRadius > 0)
        {
            cursor.EnableCursor();
        }
        else
        {
            cursor.DisableCursor();
        }

        gridCursor.SelectedItemType = itemDetails.itemType;
        cursor.SelectedItemType = itemDetails.itemType;

        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, itemDetails.itemCode);

        //如果当前选中的物体是可以carring 的 就让玩家调用对应的函数，把这个物体举起来
        if (itemDetails.canBeCarried )
        {
            Player.Instance.ShowCarriedItem(itemDetails.itemCode);
        }
        else
        {
            //如果不是，就清空当前举起来的物品 （比如从可carry 的物品 切换到 不可carry 的物品时就需要一次 clear 
            Player.Instance.ClearCarriedItem();
        }

    }


    public void ClearSelectedItem()
    {
        ClearCursors();
        inventoryBar.ClearHighlightOnInventorySlot();
        isSelected = false;
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
        //取消物品的选择时，也要清空当前carry 的item
        Player.Instance.ClearCarriedItem();
    }
}
