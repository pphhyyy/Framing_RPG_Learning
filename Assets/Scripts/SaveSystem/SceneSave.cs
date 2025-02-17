using System.Collections.Generic;
[System.Serializable]
public class SceneSave 
{
    //����������ݵ����飬��������������ʱ��ϵͳ��ʱ�����ݵġ�
    public Dictionary<string,int> intDictionary;

    public Dictionary<string, bool> boolDictionary; // �ַ����ļ��� ���ڱ�ʶ�����Ƿ�ѡ������� list ���Ƿ��ǵ�һ�μ��� 
    //һ��ͨ�� string  ��������  List<SceneItem> �ĵ䣬 ÿ��list��¼��ʾ һ�� item �� position ,
    //��Ϊ����ÿ������ ֻ�� һ�� so list ����û�б�Ҫ�� string ������ list ����ĳ� list �ͺ�
    public List<SceneItem > listSceneItem; 
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
    
    public Dictionary<string, int[]> intArrayDictionary;

    //������ҵ�λ��(�������� λ�� ����)
    public Dictionary<string,string> stringDictionary;
    public Dictionary<string,Vector3Serializable> vector3Dictionary;

    // ����/���� ʱ��,�洢��ҵ�ǰ InventoryItem ������ 
    public List<InventoryItem>[] listInvItemArray;
}
