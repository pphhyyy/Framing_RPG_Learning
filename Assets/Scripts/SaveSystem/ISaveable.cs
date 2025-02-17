
using UnityEngine;

public interface ISaveable  //����һ���ӿڣ�֮��������Ҫʵ�ִ洢���ܵĶ��󣬶��̳�����ӿ�
{
    string ISaveableUniqueID {  get; set; } 

    GameObjectSave GameObjectSave {  get; set; }

    void ISaveable_Register();

    void ISaveable_Deregister();

    GameObjectSave ISaveableSave(); //���浽�ļ�

    void ISaveableLoad(GameSave gameSave); // ���ļ�����

    void ISaveable_StoreScene(string sceneName); // �洢 

    void ISaveable_RestoreScene(string sceneName); // �ָ�

}
