
using UnityEngine;

public interface ISaveable  //定义一个接口，之后所以需要实现存储功能的对象，都继承这个接口
{
    string ISaveableUniqueID {  get; set; } 

    GameObjectSave GameObjectSave {  get; set; }

    void ISaveable_Register();

    void ISaveable_Deregister();

    GameObjectSave ISaveableSave(); //保存到文件

    void ISaveableLoad(GameSave gameSave); // 从文件加载

    void ISaveable_StoreScene(string sceneName); // 存储 

    void ISaveable_RestoreScene(string sceneName); // 恢复

}
