using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{

    [SerializeField] private Sprite blank16x16sprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlots = null;

    [HideInInspector] public GameObject inventoryTextBoxGameObject;

    public GameObject InventoryBarDraggedItem;

    private RectTransform rectTransform;

    private bool _isInventoryBarPositionBottom = true;

    public bool isInventoryBarPositionBottom { get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += InventoryUpdate;
    }

    private void InventoryUpdate(InventoryLocation location, List<InventoryItem> list)
    {
        Debug.Log(inventorySlots.Length + list.Count);
        if(location == InventoryLocation.player)
        {
            ClearInventorySlots();

            if(inventorySlots.Length > 0 && list.Count > 0)
            {
                for(int i = 0; i < inventorySlots.Length; i++)
                {

                    if (i < list.Count)
                    {
                        int itemCode = list[i].itemCode;

                        ItemDetails itemDetails = InventoryManager.Instance.GetiItemDetails(itemCode);

                        if (itemDetails != null)
                        {
                            inventorySlots[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            inventorySlots[i].textMeshProUGUI.text = list[i].itemQuantity.ToString();
                            inventorySlots[i].itemDetails = itemDetails;
                            inventorySlots[i].itemQuantity = list[i].itemQuantity;
                            SetHighlightedInventorySlot(i);
                        }
                    }

                    else
                        break;
                    
                }
            }

            


        }
    }

    private void ClearInventorySlots()
    {
        if(inventorySlots.Length > 0 )
        {
            for(int i = 0; i< inventorySlots.Length;i++)
            {
                inventorySlots[i].inventorySlotImage.sprite = blank16x16sprite;
                inventorySlots[i].textMeshProUGUI.text = "";
                inventorySlots[i].itemDetails = null;
                inventorySlots[i].itemQuantity = 0;

                SetHighlightedInventorySlot(i);
            }    
        }
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= InventoryUpdate;
    }

    private void Update()
    {
        SwitchInventoryBarPosition();
    }

    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewportPosition();

        if(playerViewportPosition.y > 0.3f && isInventoryBarPositionBottom == false)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            isInventoryBarPositionBottom = true;
        }

        if (playerViewportPosition.y < 0.3f && isInventoryBarPositionBottom == true)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            isInventoryBarPositionBottom = false;
        }
    }

    public void ClearHighlightOnInventorySlot()
    {
        if(inventorySlots.Length > 0)
        {
            for(int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].isSelected)
                {
                    inventorySlots[i].isSelected = false;
                    inventorySlots[i].inventorySlotHighLight.color = new Color(0f, 0f, 0f, 0f);

                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
                }
            }
        }
    }

    public void SetHighlightOnInventorySlot()
    {
        if(inventorySlots.Length > 0)
        {
            for(int i = 0; i < inventorySlots.Length;i++)
            {
                SetHighlightedInventorySlot(i);
            }
        }
    }

    private void SetHighlightedInventorySlot(int i)
    {
        if(inventorySlots.Length > 0 && inventorySlots[i].itemDetails != null)
        {
            if (inventorySlots[i].isSelected)
            {
                inventorySlots[i].inventorySlotHighLight.color = new Color(1f,1f,1f, 1f);    

                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player , inventorySlots[i].itemDetails.itemCode);
            }
        }
    }

    public void DestroyCurrentlyDraggedItems()
    {
        for (int i = 0; i<inventorySlots.Length;i++)
        {
            if (inventorySlots[i].draggedItem != null)
            {
                Destroy(inventorySlots[i].draggedItem);
            }
        }
    }

    public void ClearCurrentlySelectedItems()
    {
        for (int i=0; i<inventorySlots.Length; i++)
        {
            inventorySlots[i].ClearSelectedItem();
        }
    }
}
