using UnityEngine;
using Cinemachine;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    private void OnDisable()
    {
        EventHandler.AfterSceneloadEvent -= SwitchBoundingShape;
    }


    private void OnEnable()
    {
        EventHandler.AfterSceneloadEvent += SwitchBoundingShape;
    }

 
    // Update is called once per frame
    /// <summary>
    /// �л����ڶ��� ��Ļ ��Ե  ����ײ��
    /// </summary>
    private void SwitchBoundingShape()
    {
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();
        CinemachineConfiner2D cinemachineConfiner = GetComponent<CinemachineConfiner2D>();
        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

        cinemachineConfiner.InvalidateCache();//��Ϊ������Ļ��Ե����ײ�����ı䣬������Ҫ���֮ǰ��cache����
    }
}
