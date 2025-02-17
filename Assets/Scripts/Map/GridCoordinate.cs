using System;
using UnityEngine;

//��������
[System.Serializable]
public class GridCoordinate 
{
    //���� x y ����ǰ�����ڵ�ͼ�ϵ����꣬������
    public int x;
    public int y;

    public GridCoordinate(int p1, int p2)
    {
        this.x = p1;
        this.y = p2;
    }

    //�������Щ ���� ���������ڸոմ����� GridCoordinate����Ϊ������  �� �������ͣ�����ֵ�� ֱ�ӽ�����ʽ������ת��
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
