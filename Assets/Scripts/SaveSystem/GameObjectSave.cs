using System.Collections.Generic;

[System.Serializable]
public class GameObjectSave 
{
    public Dictionary<string, SceneSave> sceneData; // 这里 string 是 场景名字， 通过场景名字来读取对应场景的SceneSave 数据

    public GameObjectSave()
    {
        sceneData = new Dictionary<string, SceneSave>();
    }

    public GameObjectSave(Dictionary<string, SceneSave> sceneData)
    {
        this.sceneData = sceneData;
    }
}
