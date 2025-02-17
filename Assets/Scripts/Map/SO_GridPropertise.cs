using System.Collections.Generic;
using UnityEngine;

//��֮ǰ����Щ��SO_ ��ͷ����һ���� �����������л��������ݵ� , ÿ����ͼ����һ�� SO_GridPropertise �� ������¼ �����ͼ�ϵ�һЩ���� �� ���� ����� GridProperty


[CreateAssetMenu(fileName = "so_GridProperties" , menuName = "Scriptable Objects/Grid Properties")]
public class SO_GridPropertise : ScriptableObject
{
    public SceneName sceneName;
    public int gridWidth; // ������(���������ͼ���ĸ� �� 
    public int gridHeight;

    public int originX; // ԭ���������ͼ ���½�
    public int originY;

    [SerializeField] // �ⲿ�ֵ���������Ϸ��ʼ����ǰ�ǿյģ��ѹ��� TilemapGridProperties �Ķ�����ú� �ͻ��Զ�������������
    public List<GridProperty> gridPropertyList; // ���һϵ�� GridProperty �� �б� �� �����ǵ�ǰ�����������������
}
