[System.Serializable]
public class SceneItem //������ʾ����������ģ���¼item �� ���� code λ�� ��֮�����ͨ�����л��洢����
{
    public int itemCode;
    public Vector3Serializable position;
    public string itemName;

    public SceneItem()
    {
        position = new Vector3Serializable();
    }
}
