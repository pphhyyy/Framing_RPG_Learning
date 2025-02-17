using System.Collections.Generic;
using UnityEngine;

//和之前的那些以SO_ 开头的类一样， 是用来做序列化配置数据的 , 每个地图都有一个 SO_GridPropertise ， 用来记录 这个地图上的一些属性 和 所有 网格的 GridProperty


[CreateAssetMenu(fileName = "so_GridProperties" , menuName = "Scriptable Objects/Grid Properties")]
public class SO_GridPropertise : ScriptableObject
{
    public SceneName sceneName;
    public int gridWidth; // 这里是(整个网格地图）的高 宽 
    public int gridHeight;

    public int originX; // 原点在网格地图 左下角
    public int originY;

    [SerializeField] // 这部分的数据在游戏开始运行前是空的，把挂有 TilemapGridProperties 的对象禁用后 就会自动填充里面的内容
    public List<GridProperty> gridPropertyList; // 存放一系列 GridProperty 的 列表 。 里面是当前场景所有网格的数据
}
