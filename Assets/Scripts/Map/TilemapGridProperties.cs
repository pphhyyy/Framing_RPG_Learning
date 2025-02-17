using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

//用来从场景上画好的 TileMap 中读取我们需要的 GridProperty  ， 这里 每一个类型的 boolTileMap 都会挂载一个这个脚本 用来遍历整个 地图上 所有网格 的属性
[ExecuteAlways] // 这个类也是编辑器状态下才能使用
public class TilemapGridProperties : MonoBehaviour
{
#if UNITY_EDITOR // 保证下面的代码只有在编辑器中才编译
    private Tilemap tilemap;
    private Grid grid;
    [SerializeField] private SO_GridPropertise gridPropertise = null; // 从当前所在场景上，获得这个场景的 各种属性 和 场景上所有网格 的 GridProperty
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.diggable;

    private void OnEnable()
    {
        if(!Application.IsPlaying(gameObject)) // 如果当前物体没有在游戏状态（就算在编辑状态）
        {
            tilemap = GetComponent<Tilemap>();
            
            if( gridPropertise != null )
            {
                 gridPropertise.gridPropertyList.Clear(); //如果OnEnable的时候 当前组件身上已经有gridPropertise了 清空当前场景上所有网格的 GridProperty
            }
        }
    }

    private void OnDisable()
    {
        if(!Application.IsPlaying(gameObject) )
        {
            UpdateGridProperties(); // 这里只有把对象 禁用的时候 才会调用 UpdateGridProperties 函数 , 生成对应的 gridPropertyList 数据

            if ( gridPropertise != null )
            {
                EditorUtility.SetDirty( gridPropertise );
            }
        }
    }

    private void UpdateGridProperties()
    {
        //Debug.Log(gameObject.name + "约束前" +  tilemap.cellBounds.min.x + "::" + tilemap.cellBounds.max);

        tilemap.CompressBounds(); // 压缩边界， 通过检测目前场景上已经画上东西的 grid 把边界限制到刚好能包住这些有内容的 grid,默认状态下 tilemap 非常大

        //Debug.Log(gameObject.name + "约束后" + tilemap.cellBounds.min + "::" + tilemap.cellBounds.max);

        if (!Application.IsPlaying(gameObject))
        {
            if(gridPropertise != null )
            {
                Vector3Int startCell = tilemap.cellBounds.min;  //左下角
                Vector3Int endCell = tilemap.cellBounds.max;    //右上角

                for (int x = startCell.x; x < endCell.x; x++)
                {
                    for (int y = startCell.y ; y < endCell.y; y++)
                    {
                       //遍历整个tilemap
                       TileBase tile = tilemap.GetTile(new Vector3Int(x, y , 0 )); // 从整个 tilemap上，根据x y 坐标 获取单个 tile 

                       if ( tile != null )     //如果当前坐标上 取到了 tile ，说明这个格子上有画东西 
                       {
                           //给 对应 grid 添加  gridProperty ， 并存储在 gridPropertyList 中
                           gridPropertise.gridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), gridBoolProperty, true));
                       }

                        
                    }
                }
            }
        }
    }

    private void Update()
    {
        if(!Application.IsPlaying(gameObject))
        {
            Debug.Log("DISABLE PROPERTY TILEMAPS");//提醒你，画完bool 图以后 要禁用 bool图的对象 ，不禁用就会一直打印  "DISABLE PROPERTY TILEMAPS"
        }
    }
#endif
}
