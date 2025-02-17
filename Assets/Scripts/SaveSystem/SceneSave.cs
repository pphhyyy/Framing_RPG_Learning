using System.Collections.Generic;
[System.Serializable]
public class SceneSave 
{
    //存放整形数据的数组，这里是用来保存时间系统的时间数据的。
    public Dictionary<string,int> intDictionary;

    public Dictionary<string, bool> boolDictionary; // 字符串的键， 用于标识我们是否选择了这个 list ，是否是第一次加载 
    //一个通过 string  来索引的  List<SceneItem> 的典， 每个list记录表示 一类 item 的 position ,
    //因为这里每个场景 只有 一个 so list 所以没有必要用 string 来索引 list 这里改成 list 就好
    public List<SceneItem > listSceneItem; 
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
    
    public Dictionary<string, int[]> intArrayDictionary;

    //保存玩家的位置(包括场景 位置 面向)
    public Dictionary<string,string> stringDictionary;
    public Dictionary<string,Vector3Serializable> vector3Dictionary;

    // 保存/加载 时候,存储玩家当前 InventoryItem 的数据 
    public List<InventoryItem>[] listInvItemArray;
}
