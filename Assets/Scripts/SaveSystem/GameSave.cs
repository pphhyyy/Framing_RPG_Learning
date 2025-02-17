using System.Collections.Generic;


[System.Serializable]
public class GameSave 
{
    // string Key ->>> GUID gameobject ID
    public Dictionary<string, GameObjectSave> gameObjectData;

    public GameSave()
    {
        gameObjectData = new Dictionary<string, GameObjectSave>();  
    }
}
