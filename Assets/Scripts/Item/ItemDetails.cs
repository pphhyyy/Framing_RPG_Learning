
using UnityEngine;

[System.Serializable]
public class ItemDetails 
{
    public int itemCode;
    public ItemType itemType;
    public string itemDescription;
    public Sprite itemSprite;
    public string itemLongDescription;
    public short itemUseGridRadius;//��ҿ����ڶ�Զ�ľ��������Ʒ��������Ϊ��λ��
    public float itemUseRadius; //��ҿ����ڶ�Զ�ľ��������Ʒ���� unity������λ Ϊ��λ��
    public bool isStartingIte;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;
    public bool canBeCarried;
}
