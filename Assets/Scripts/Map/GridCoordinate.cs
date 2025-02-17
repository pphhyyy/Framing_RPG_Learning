using System;
using UnityEngine;

//网格坐标
[System.Serializable]
public class GridCoordinate 
{
    //这里 x y 代表当前网格在地图上的坐标，是整数
    public int x;
    public int y;

    public GridCoordinate(int p1, int p2)
    {
        this.x = p1;
        this.y = p2;
    }

    //下面的这些 操作 允许我们在刚刚创建的 GridCoordinate（作为参数）  和 其他类型（返回值） 直接进行显式的类型转换
    public static explicit operator Vector2(GridCoordinate gridCoordinate)
    {
        return new Vector2((float)gridCoordinate.x , (float)gridCoordinate.y);
    }

    public static explicit operator Vector2Int(GridCoordinate gridCoordinate)
    {
        return new Vector2Int(gridCoordinate.x, gridCoordinate.y);
    }

    public static explicit operator Vector3(GridCoordinate gridCoordinate)
    {
        return new Vector3((float)gridCoordinate.x, (float)gridCoordinate.y, 0f);
    }

    public static explicit operator Vector3Int(GridCoordinate gridCoordinate)
    {
        return new Vector3Int(gridCoordinate.x, gridCoordinate.y, 0);
    }
}
