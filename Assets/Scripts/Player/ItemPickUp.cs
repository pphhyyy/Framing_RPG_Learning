using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>(); 

        if(item != null )
        {
            ItemDetails itemDetails = InventoryManager.Instance.GetiItemDetails(item.ItemCode);

            if(itemDetails.canBePickedUp == true)
            {
                //�����Ҿ����������п��Լ���ı�ǩ���͵��û�ݻ�ԭ������Ǹ� AddItem
                InventoryManager.Instance.AddItem(InventoryLocation.player , item , collision.gameObject);
            }
        }
        
    }
}
