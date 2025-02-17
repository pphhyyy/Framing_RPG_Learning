using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class Cursor : MonoBehaviour
{
    private Canvas canvas;

    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite transparentCursorSprite = null;
    [SerializeField] private GridCursor gridCursor = null;


    private bool _cursorIsEnable = false;
    public bool CursorIsEnable { get => _cursorIsEnable; set => _cursorIsEnable = value; }

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }


    private float _itemUseRadius = 0f;
    public float ItemUseRadius { get => _itemUseRadius; set => _itemUseRadius = value; }

    private ItemType _selectedItemType ;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (CursorIsEnable)
        {
            DisplayCursor();
        }
    }

    

    private void DisplayCursor()
    {
        
            Vector3 cursorWorldPosition = GetWorldPositionForCursor();

        Debug.Log("cursorWorldPosition : " + cursorWorldPosition + "Player" + Player.Instance.GetPlayerCenterPosition());
            SetCursorValidity(cursorWorldPosition, Player.Instance.GetPlayerCenterPosition()); // 设置光标的有效范围

            cursorRectTransform.position = GetRectTransformPositionForCursor();

           
    }

    private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition)
    {
        SetCursorToValid();

        if(
           cursorPosition.x > (playerPosition.x + ItemUseRadius /2f ) && cursorPosition.y > (playerPosition.y + ItemUseRadius / 2f)
           ||
           cursorPosition.x < (playerPosition.x - ItemUseRadius / 2f) && cursorPosition.y > (playerPosition.y + ItemUseRadius / 2f)
           ||
           cursorPosition.x < (playerPosition.x - ItemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - ItemUseRadius / 2f)
           ||
           cursorPosition.x > (playerPosition.x + ItemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - ItemUseRadius / 2f)
           )
        {
            SetCursorToInvalid();
            Debug.Log("第一类错误");
            return;
        }

        if(Mathf.Abs(cursorPosition.x - playerPosition.x) > ItemUseRadius  || Mathf.Abs(cursorPosition.y - playerPosition.y) > ItemUseRadius)
        {
            SetCursorToInvalid();
            Debug.Log("第二类错误");
            
            return;
        }

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if( itemDetails == null )
        {
            SetCursorToInvalid();
            Debug.Log("第三类错误");
            return;
        }

        switch (itemDetails.itemType)
        {
            case ItemType.Watering_tool:
            case ItemType.Hoeing_tool:
            case ItemType.Chopping_tool:
            case ItemType.Breaking_tool:
            case ItemType.Reaping_tool:
            case ItemType.Collecting_tool:
                if (!SetCursorValidityTool(cursorPosition, playerPosition, itemDetails))
                {
                    SetCursorToInvalid() ;
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

    private bool SetCursorValidityTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        switch(itemDetails.itemType)
        {
            case ItemType.Reaping_tool:
                return SetCursorValidityReapingTool(cursorPosition, playerPosition, itemDetails);

            default: 
                return false;   
        }
    }

    private bool SetCursorValidityReapingTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        List<Item> itemList = new List<Item>();

        if(HelperMethods.GetComponentsAtCursorLocation<Item>(out itemList , cursorPosition))
        {
            if(itemList.Count != 0)
            {
                foreach(Item item in itemList)
                {
                    if(InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }


    public void DisableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 0f);
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

        gridCursor.DisableCursor();
    }

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = transparentCursorSprite;
        CursorPositionIsValid = false;
        gridCursor.EnableCursor();
    }


    

    public Vector3 GetWorldPositionForCursor()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

        //Debug.Log("worldPosition: " + worldPosition);
        return worldPosition;
    }

    private Vector3 GetRectTransformPositionForCursor()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x , Input.mousePosition.y);
        return RectTransformUtility.PixelAdjustPoint(screenPosition, cursorRectTransform, canvas);
    }

}
