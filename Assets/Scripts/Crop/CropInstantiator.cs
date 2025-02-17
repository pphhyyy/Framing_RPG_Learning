using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 附加在 crop 预设体中 以设置 grid 属性 字典的数值
/// </summary>
public class CropInstantiator : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private int daysSinceDug = -1;
    [SerializeField] private int daysSinceWatered = -1;
    [ItemCodeDescription]
    [SerializeField] private int seedItemCode = 0;
    [SerializeField] private int growthDays = 0;

    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabsEvent -= InstantiateCropPrefabs;

    }

    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabsEvent += InstantiateCropPrefabs;

    }

    private void InstantiateCropPrefabs()
    {
        Debug.Log("初始化场景中已有的crop ！");
        //从当前挂载的物体上 获取 grid 对象
        grid = GameObject.FindObjectOfType<Grid>();

        //从当前的crop 上 获取 grid 的 位置 
        Vector3Int cropGridPosition = grid.WorldToCell(transform.position);

        //设置 crop grid 的属性 
        SetCropGridProperties(cropGridPosition);

        //因为是 初始化器（intantiator) 所以设置完 属性以后 就可以销毁这个 游戏对象了
        Destroy(gameObject);
    }

    private void SetCropGridProperties(Vector3Int cropGridPosition)
    {
        GridPropertyDetails gridPropertyDetails;

        //从当前 cropgrid 坐标 获取 grid 的属性细节
        gridPropertyDetails = GridPropertIesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

        if (gridPropertyDetails == null) // 如果当前的 cropGridPosition 中没有找到 gridPropertyDetails 就自己实例化一个 
        {
            Debug.Log("debug");
            gridPropertyDetails = new GridPropertyDetails();
        }
        
        // 将 CropInstantiator 中设置的 gridPropertyDetails
        // 赋值给 我们从 GridPropertIesManager 中，通过 grid 坐标获得的 gridPropertyDetails
        gridPropertyDetails.daysSinceDug = daysSinceDug;
        gridPropertyDetails.daySinceWatered = daysSinceWatered;
        gridPropertyDetails.seedItemCode = seedItemCode;
        gridPropertyDetails.growthDays = growthDays;

        //将 cropgrid 的各项参数 回传给 GridPropertIesManager ，这一步做完，初始化就完成了 
        GridPropertIesManager.Instance.SetGridPropertyDetials(cropGridPosition.x,cropGridPosition.y , gridPropertyDetails);

    }
}