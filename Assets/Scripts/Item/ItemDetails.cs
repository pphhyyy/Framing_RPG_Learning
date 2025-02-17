
using UnityEngine;

[System.Serializable]
public class ItemDetails 
{
    public int itemCode;
    public ItemType itemType;
    public string itemDescription;
    public Sprite itemSprite;
    public string itemLongDescription;
    public short itemUseGridRadius;//玩家可以在多远的距离捡起物品（以网格为单位）
    public float itemUseRadius; //玩家可以在多远的距离捡起物品（以 unity基本单位 为单位）
    public bool isStartingIte;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;
    public bool canBeCarried;
}
