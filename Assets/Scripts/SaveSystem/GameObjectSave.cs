using System.Collections.Generic;

[System.Serializable]
public class GameObjectSave 
{
    public Dictionary<string, SceneSave> sceneData; // ���� string �� �������֣� ͨ��������������ȡ��Ӧ������SceneSave ����

    public GameObjectSave()
    {
        sceneData = new Dictionary<string, SceneSave>();
    }

    public GameObjectSave(Dictionary<string, SceneSave> sceneData)
    {
        this.sceneData = sceneData;
    }
}
