[System.Serializable]
public class Vector3Serializable 
//�����Լ��Ŀ����л���Vector3 �� �����㱣�����λ����Ϣ����Ϊ unity �Դ���Vector3 �ǲ������л���
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
