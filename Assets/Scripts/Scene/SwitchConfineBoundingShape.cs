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
    /// 切换用于定义 屏幕 边缘  的碰撞器
    /// </summary>
    private void SwitchBoundingShape()
    {
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();
        CinemachineConfiner2D cinemachineConfiner = GetComponent<CinemachineConfiner2D>();
        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

        cinemachineConfiner.InvalidateCache();//因为代表屏幕边缘的碰撞器被改变，这里需要清楚之前的cache缓存
    }
}
