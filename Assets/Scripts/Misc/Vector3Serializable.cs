[System.Serializable]
public class Vector3Serializable 
//建立自己的可序列化的Vector3 类 ，方便保存各种位置信息，因为 unity 自带的Vector3 是不可序列化的
{
    public float x, y, z;

    public Vector3Serializable(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Serializable()
    {

    }
}
