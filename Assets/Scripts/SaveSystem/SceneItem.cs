[System.Serializable]
public class SceneItem //用来表示场景中物体的，记录item 的 名字 code 位置 ，之后可以通过序列化存储下来
{
    public int itemCode;
    public Vector3Serializable position;
    public string itemName;

    public SceneItem()
    {
        position = new Vector3Serializable();
    }
}
