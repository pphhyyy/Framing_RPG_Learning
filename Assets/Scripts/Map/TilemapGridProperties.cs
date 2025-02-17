using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

//�����ӳ����ϻ��õ� TileMap �ж�ȡ������Ҫ�� GridProperty  �� ���� ÿһ�����͵� boolTileMap �������һ������ű� ������������ ��ͼ�� �������� ������
[ExecuteAlways] // �����Ҳ�Ǳ༭��״̬�²���ʹ��
public class TilemapGridProperties : MonoBehaviour
{
#if UNITY_EDITOR // ��֤����Ĵ���ֻ���ڱ༭���вű���
    private Tilemap tilemap;
    private Grid grid;
    [SerializeField] private SO_GridPropertise gridPropertise = null; // �ӵ�ǰ���ڳ����ϣ������������� �������� �� �������������� �� GridProperty
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.diggable;

    private void OnEnable()
    {
        if(!Application.IsPlaying(gameObject)) // �����ǰ����û������Ϸ״̬�������ڱ༭״̬��
        {
            tilemap = GetComponent<Tilemap>();
            
            if( gridPropertise != null )
            {
                 gridPropertise.gridPropertyList.Clear(); //���OnEnable��ʱ�� ��ǰ��������Ѿ���gridPropertise�� ��յ�ǰ��������������� GridProperty
            }
        }
    }

    private void OnDisable()
    {
        if(!Application.IsPlaying(gameObject) )
        {
            UpdateGridProperties(); // ����ֻ�аѶ��� ���õ�ʱ�� �Ż���� UpdateGridProperties ���� , ���ɶ�Ӧ�� gridPropertyList ����

            if ( gridPropertise != null )
            {
                EditorUtility.SetDirty( gridPropertise );
            }
        }
    }

    private void UpdateGridProperties()
    {
        //Debug.Log(gameObject.name + "Լ��ǰ" +  tilemap.cellBounds.min.x + "::" + tilemap.cellBounds.max);

        tilemap.CompressBounds(); // ѹ���߽磬 ͨ�����Ŀǰ�������Ѿ����϶����� grid �ѱ߽����Ƶ��պ��ܰ�ס��Щ�����ݵ� grid,Ĭ��״̬�� tilemap �ǳ���

        //Debug.Log(gameObject.name + "Լ����" + tilemap.cellBounds.min + "::" + tilemap.cellBounds.max);

        if (!Application.IsPlaying(gameObject))
        {
            if(gridPropertise != null )
            {
                Vector3Int startCell = tilemap.cellBounds.min;  //���½�
                Vector3Int endCell = tilemap.cellBounds.max;    //���Ͻ�

                for (int x = startCell.x; x < endCell.x; x++)
                {
                    for (int y = startCell.y ; y < endCell.y; y++)
                    {
                       //��������tilemap
                       TileBase tile = tilemap.GetTile(new Vector3Int(x, y , 0 )); // ������ tilemap�ϣ�����x y ���� ��ȡ���� tile 

                       if ( tile != null )     //�����ǰ������ ȡ���� tile ��˵������������л����� 
                       {
                           //�� ��Ӧ grid ���  gridProperty �� ���洢�� gridPropertyList ��
                           gridPropertise.gridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), gridBoolProperty, true));
                       }

                        
                    }
                }
            }
        }
    }

    private void Update()
    {
        if(!Application.IsPlaying(gameObject))
        {
            Debug.Log("DISABLE PROPERTY TILEMAPS");//�����㣬����bool ͼ�Ժ� Ҫ���� boolͼ�Ķ��� �������þͻ�һֱ��ӡ  "DISABLE PROPERTY TILEMAPS"
        }
    }
#endif
}
