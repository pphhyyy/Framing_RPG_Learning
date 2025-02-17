using System;
using UnityEngine;

[System.Serializable]
public class CropDetails
{
    //ׯ�ڵ�ϸ��

    [ItemCodeDescription]
    public int seedItemCode;  //��ʶ��ǰcrop �����ӵ� itemcode 
    public int[] growthDay;  //���������ÿ���׶���Ҫ���ĵ����� �������ӵ���ѿ 2�� �� ��ѿ������Ҷ�� 20�� ֮��ģ�
    public GameObject[] growthPrefab;   //�� growthDay һ�� ���ڴ��ÿ���׶������prefab �������������������Ҫ��prefab �ͻ᲻ͬ
    public Sprite[] growthSprite;       //�� growthDay һ�� ���ڴ��ÿ���׶���ʾʱ��sprite 
    public Season[] seasons;            // �ض����� �� ��Щ����
    public Sprite harvestedSprite;      //�ջ��Ժ�������� sprite 


    [ItemCodeDescription]
    public int harvestedTransformItemCode; //һЩ���� ������ �� ���ջ� harvested �Ժ���ܱ����׮ ����ʱ�� ���巢���ı� ��Ӧ��item code Ҳ��ı�
    public bool hideCropBeforeHarvestedAnimation;
    public bool disableCropCollidersBeforeHarvestedAnimation;

    public bool isHarvestedAnimation;   //��ժʱ�Ƿ��ж��� 
    public bool isHarvestActionEffect = false;  // �Ƿ�����Ч 
    public bool spawnCropProducedAtPlayerPosition;
    public HarvestActionEffect harvestActionEffect;

    [ItemCodeDescription]
    public int[] harvestToolItemCode;  //�������ʹ����Щ tool ������ �ո� �� �������� tool �� item code 
    public int[] requiredHarvestActions; // ��Ҫ tool ���ٴ� action ���ܽ��� Harvest
    [ItemCodeDescription]
    public int[] cropProducedItemCode;  //��� Harvest ���ջ�� item �� itemcode ���翳���Ժ�ֹ�����ľ�ģ�Ҳ���ܻ�����ϵ��ɹ�
    public int[] cropProducedMinQuantity;       //��õ��������
    public int[] cropProducedMaxQuantity;       //��õ���С����

    public int daysToRegrow;                //һЩ�����ڽ���һ�� Harvest �� ����������ܽ�����һ�� Harvest 


    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public
        int RequiredHarvestActionsForTool(int toolItemCode)
    {
        //���� harvestToolItemCode ���� ����ǰʹ�� tool �� itemcode �Ƿ�� harvestToolItemCode �����м�¼������һ�� ���һ�� �ͷ������ requiredHarvestActions
        for (int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if (harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}
