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
    [HideInInspector] public bool isSelected = false; // ��ǰ��Ʒ���Ƿ�ѡ��

    [SerializeField] private int slotNumber = 0; //��ǰ��Ʒ����Ʒ���ڵ�λ��

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
        //������Ҫ�ڳɼ���������Ժ󣬲����ٳ�����Ѱ���ж�Ӧ��ǩ�� ���� ,ItemsParentTransform ���Ƕ�Ӧ�����ϵ� item �������ڲ���һ���item 
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

            //��ʼ�϶�һ������ʱ����������Ʒ���ϸ�����ʾ�������
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
                //�϶����� ��Ʒ�� �� 
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>().slotNumber; //�϶���Ŀ�ĵ�����Ʒ���е�λ��

                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

                DestroyInventoryTextBox();

                //�����϶�ȡ����Ʒ���ϵĸ�����ʾ
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

            //����ӳ�� cursorpositionisVaild �Ժ� �Ͳ��� �����ж��Ƿ� canDropItem ��
            // //���˴��Ƿ���Է�����Ʒ
            // Vector3Int gridPosition = GridPropertIesManager.Instance.grid.WorldToCell(worldPosition);
            //// Debug.Log(gridPosition);
            // GridPropertyDetails gridPropertyDetails = GridPropertIesManager.Instance.GetGridPropertyDetails(gridPosition.x, gridPosition.y);


            if (gridCursor.CursorPositionIsValid) // �����ǰ����ӵ�У��ɷ�����Ʒ�� ������ �� �Ϳ��Խ��з���
            {
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x , Input.mousePosition.y , -mainCamera.transform.position.z));
                GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(worldPosition.x,worldPosition.y - Settings.gridCellSize/2f , worldPosition.z), Quaternion.identity, parentItem);
                Item item = itemGameObject.GetComponent<Item>();
                item.ItemCode = itemDetails.itemCode;

                InventoryManager.Instance.RemoveItem(InventoryLocation.player , item.ItemCode);

                //�������Ʒ�ѳ��Ժ󣬵�ǰ��Ʒ����Ʒ�����Ѿ�û��ʣ�࣬�Ϳ������������ı�־
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

        //�����ǰѡ�е������ǿ���carring �� ������ҵ��ö�Ӧ�ĺ�������������������
        if (itemDetails.canBeCarried )
        {
            Player.Instance.ShowCarriedItem(itemDetails.itemCode);
        }
        else
        {
            //������ǣ�����յ�ǰ����������Ʒ ������ӿ�carry ����Ʒ �л��� ����carry ����Ʒʱ����Ҫһ�� clear 
            Player.Instance.ClearCarriedItem();
        }

    }


    public void ClearSelectedItem()
    {
        ClearCursors();
        inventoryBar.ClearHighlightOnInventorySlot();
        isSelected = false;
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
        //ȡ����Ʒ��ѡ��ʱ��ҲҪ��յ�ǰcarry ��item
        Player.Instance.ClearCarriedItem();
    }
}
