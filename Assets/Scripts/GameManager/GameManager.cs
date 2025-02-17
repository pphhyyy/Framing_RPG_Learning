using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    // �� ��һ֮֡ǰ��ʼ����
    protected override void Awake()
    {
        base.Awake();
        // ����ȫ���ֱ���Ϊ 1920*1080
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow, 0);
    }
}
