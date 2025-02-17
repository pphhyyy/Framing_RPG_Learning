using System;
using UnityEngine;

[System.Serializable]
public class CropDetails
{
    //庄稼的细节

    [ItemCodeDescription]
    public int seedItemCode;  //标识当前crop 的种子的 itemcode 
    public int[] growthDay;  //存放种子在每个阶段需要消耗的天数 （如种子到发芽 2天 ， 发芽到长出叶子 20天 之类的）
    public GameObject[] growthPrefab;   //和 growthDay 一样 用于存放每个阶段物体的prefab 比如从种子正成树，需要的prefab 就会不同
    public Sprite[] growthSprite;       //和 growthDay 一样 用于存放每个阶段显示时的sprite 
    public Season[] seasons;            // 特定季节 种 这些作物
    public Sprite harvestedSprite;      //收获以后获得物体的 sprite 


    [ItemCodeDescription]
    public int harvestedTransformItemCode; //一些东西 比如树 在 “收获” harvested 以后可能变成树桩 ，这时候 物体发生改变 对应的item code 也会改变
    public bool hideCropBeforeHarvestedAnimation;
    public bool disableCropCollidersBeforeHarvestedAnimation;

    public bool isHarvestedAnimation;   //采摘时是否有动画 
    public bool isHarvestActionEffect = false;  // 是否有特效 
    public bool spawnCropProducedAtPlayerPosition;
    public HarvestActionEffect harvestActionEffect;

    [ItemCodeDescription]
    public int[] harvestToolItemCode;  //允许玩家使用那些 tool 来进行 收割 ， 这里填入 tool 的 item code 
    public int[] requiredHarvestActions; // 需要 tool 多少次 action 才能进行 Harvest
    [ItemCodeDescription]
    public int[] cropProducedItemCode;  //完成 Harvest 后收获的 item 的 itemcode 比如砍树以后不止会掉下木材，也可能获得树上的松果
    public int[] cropProducedMinQuantity;       //获得的最大数量
    public int[] cropProducedMaxQuantity;       //获得的最小数量

    public int daysToRegrow;                //一些作物在进行一次 Harvest 后 间隔多少天能进行下一次 Harvest 


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
        //遍历 harvestToolItemCode 数组 看当前使用 tool 的 itemcode 是否和 harvestToolItemCode 数组中记录的内容一致 如果一致 就返回这个 requiredHarvestActions
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
