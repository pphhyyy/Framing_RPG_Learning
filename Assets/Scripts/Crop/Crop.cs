
using System.Collections;
using UnityEngine;

public class Crop : MonoBehaviour
{

    private int harvestActionCount = 0; // 需要多少次 收获动作 才能收获这个 crop 

    [Tooltip("This should be populated from child transform gameobject showing harvest effect spawn point")]
    [SerializeField] private Transform harvestActionEffectTransform = null;

    [Tooltip("this should be populated from child gameobject")]
    [SerializeField] private SpriteRenderer cropHarvestedSpriteRender;
    [HideInInspector]
    public Vector2Int cropGridPosition;

    public void ProcessToolAction(ItemDetails equipitemDetails , bool isToolRight , bool isToolLeft, bool isToolDown, bool isToolUp )
    {
        Debug.Log("调用ProcessToolAction");
        GridPropertyDetails gridPropertyDetails = GridPropertIesManager.Instance.GetGridPropertyDetails(cropGridPosition.x , cropGridPosition.y);
        if (gridPropertyDetails == null)
        {
            Debug.Log("1");
            return;
        }
            
        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if (seedItemDetails == null)
        {
            Debug.Log("2");
            return;
        }
        CropDetails cropDetails = GridPropertIesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if(cropDetails == null)
        {
            Debug.Log("3" + "equipitemDetails.itemCode::" + equipitemDetails.itemCode);
            return;
        }

        Animator animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            if(isToolRight || isToolUp)
            {
                animator.SetTrigger("usetoolright");
            }

            else if (isToolLeft || isToolDown)
            {
                animator.SetTrigger("usetoolleft");
            }
        }
        // 斧头工具砍树的时候 出现的落叶特效
        if (cropDetails.isHarvestActionEffect)
        {
            EventHandler.CallHarvestActionEffectEvent (harvestActionEffectTransform.position, cropDetails.harvestActionEffect);
        }


        int requireHarvestAction = cropDetails.RequiredHarvestActionsForTool(equipitemDetails.itemCode);
        Debug.Log("requireHarvestAction::" + requireHarvestAction);
        if (requireHarvestAction == -1)
           return;

        harvestActionCount += 1;
        Debug.Log("harvestActionCount: " + harvestActionCount + ":::requireHarvestAction" + requireHarvestAction);
        if (harvestActionCount >= requireHarvestAction)
            HarvestCrop(isToolRight , isToolUp , cropDetails, gridPropertyDetails , animator);
    }

    private void HarvestCrop(bool isUsingToolRight , bool isUsingToolUp ,   CropDetails cropDetails, GridPropertyDetails gridPropertyDetails , Animator animator)
    {
        if(cropDetails.isHarvestedAnimation && animator != null)
        {
            if(cropDetails.harvestedSprite != null)
            {
                if(cropHarvestedSpriteRender != null)
                {
                    cropHarvestedSpriteRender.sprite = cropDetails.harvestedSprite;
                }
            }

            if (isUsingToolRight || isUsingToolUp)
            {
                animator.SetTrigger("harvestright");
            }

            else 
            {
                animator.SetTrigger("harvestleft");
            }
        }


        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daySinceWatered = -1;

        if(cropDetails.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        ///这里是为了解决，用十字镐挖石头后，出现的石头上浮动画(表示获得了这个石头）中带有碰撞体，导致如果角色挖的时候
        ///站在石头上方格子处，就会被弹开,所以在这里设置一下，把出现的动画上浮的物体身上的碰撞体失去enable 
        if (cropDetails.disableCropCollidersBeforeHarvestedAnimation )
        {
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();    
            foreach(Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false; 
            }
        }


        GridPropertIesManager.Instance.SetGridPropertyDetials(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        if(cropDetails.isHarvestedAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestActionsAffterAnimation(cropDetails, gridPropertyDetails, animator));
        }

        else
        {
            HarvestActions(cropDetails, gridPropertyDetails);
        }
      
    }

    private IEnumerator ProcessHarvestActionsAffterAnimation(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        while(!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;  
        }

        HarvestActions(cropDetails , gridPropertyDetails);
    }

    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails);

        if (cropDetails.harvestedTransformItemCode > 0)
        {
            CreatHarvestedTransformCrop(cropDetails,gridPropertyDetails);
        }
        Debug.Log("Destroy");
        Destroy(gameObject);
    }


    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        Debug.Log("cropDetails.cropProducedItemCode.Length" + cropDetails.cropProducedItemCode.Length);
        for(int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            int cropsToProduce;

            if (cropDetails.cropProducedMinQuantity[i] == cropDetails.cropProducedMaxQuantity[i] || cropDetails.cropProducedMaxQuantity[i] < cropDetails.cropProducedMinQuantity[i])
            { 
                cropsToProduce = cropDetails.cropProducedMinQuantity[i];
            }
            else
            {
                cropsToProduce = Random.Range(cropDetails.cropProducedMinQuantity[i] , cropDetails.cropProducedMaxQuantity[i] + 1 ); 
            }


            Debug.Log("cropsToProduce: " + cropsToProduce);

            for(int j = 0; j < cropsToProduce;  j++)
            {
                Vector3 spawnPosition;
                if(cropDetails.spawnCropProducedAtPlayerPosition)
                {
                    InventoryManager.Instance.AddItem(InventoryLocation.player , cropDetails.cropProducedItemCode[i]);
                }
                else
                {
                    spawnPosition = new Vector3(transform.position.x + Random.Range(-1f,1f) , transform.position.y + Random.Range(-1f, 1f) , 0f );
                    SceneItemManger.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i] , spawnPosition);
                }
            }
        }
    }


    private void CreatHarvestedTransformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daySinceWatered = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;

        GridPropertIesManager.Instance.SetGridPropertyDetials(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        GridPropertIesManager.Instance.DisplayPlantedCrops(gridPropertyDetails);
    }


}
