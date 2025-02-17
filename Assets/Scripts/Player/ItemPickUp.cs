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
                //如果玩家经过的物体有可以捡起的标签，就调用会摧毁原物体的那个 AddItem
                InventoryManager.Instance.AddItem(InventoryLocation.player , item , collision.gameObject);
            }
        }
        
    }
}
