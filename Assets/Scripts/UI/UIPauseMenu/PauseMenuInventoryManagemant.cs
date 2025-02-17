using System.Collections.Generic;
using UnityEngine;

public class PauseMenuInventoryManagemant : MonoBehaviour
{
    [SerializeField] private PauseMenuInventoryManagemantSlot[] inventoryManagementSlot = null;

    public GameObject inventoryManagementDraggedItemPrefab;

    [SerializeField] private Sprite transparent16x16 = null;

    [HideInInspector] public GameObject inventoryTextBoxGameobject;


    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += PopulatePlayerInventory;

        if (InventoryManager.Instance != null)
        {
            PopulatePlayerInventory(InventoryLocation.player, InventoryManager.Instance.InventoryLists[(int)InventoryLocation.player]);
        }
    }



    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= PopulatePlayerInventory;
        DestoryInventoryTextBoxGameobject();
    }

    public void DestoryInventoryTextBoxGameobject()
    {
        if(inventoryTextBoxGameobject != null)
        {
            Destroy(inventoryTextBoxGameobject);
        }
    }

    public void DestroyyCurrentlyDraggedItems()
    {
        for(int i = 0; i < InventoryManager.Instance.InventoryLists[(int)InventoryLocation.player].Count; i++)
        {
            if (inventoryManagementSlot[i].draggedItem != null)
            {
                Destroy(inventoryManagementSlot[i].draggedItem);
            }
        }
    }

    private void PopulatePlayerInventory(InventoryLocation inventoryLocation, List<InventoryItem> playeInventoryList)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            InitialiseInventoryManagementSlots();

            for (int i = 0; i < InventoryManager.Instance.InventoryLists[(int)InventoryLocation.player].Count;i++)
            {
                //��ȡ inventory �� item �� detials 
                inventoryManagementSlot[i].itemDetails = InventoryManager.Instance.GetItemDetails(playeInventoryList[i].itemCode);
                inventoryManagementSlot[i].itemQuantity = playeInventoryList[i].itemQuantity;

                if (inventoryManagementSlot[i].itemDetails != null)
                {
                    // ���� inventory management slot  �� (����) ������ (ͼƬ)spirte ��ʾ
                    inventoryManagementSlot[i].inventoryManagementSlotImage.sprite = inventoryManagementSlot[i].itemDetails.itemSprite;
                    inventoryManagementSlot[i].textMeshProUGUI.text = inventoryManagementSlot[i].itemQuantity.ToString();
                }
            }
        }
    }

    private void InitialiseInventoryManagementSlots()
    {
       //�����ǰ inventory �� slot 
       for (int i = 0; i<Settings.playerInitialInventoryCapacity;i++)
        {
            inventoryManagementSlot[i].greyedOutImageGo.SetActive(false);
            inventoryManagementSlot[i].itemDetails = null;
            inventoryManagementSlot[i].itemQuantity = 0;
            inventoryManagementSlot[i].inventoryManagementSlotImage.sprite = transparent16x16;
            inventoryManagementSlot[i].textMeshProUGUI.text = "";
        }
       //Grey Out Unavailable Slots
       for (int i = InventoryManager.Instance.inventoryListCapacityIntArray[(int)InventoryLocation.player]; i < Settings.playerMaximumInventoryCapacity;i++)
        {
            inventoryManagementSlot[i].greyedOutImageGo.SetActive(true);
        }

    }
}


