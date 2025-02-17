using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    // 在 第一帧之前开始运行
    protected override void Awake()
    {
        base.Awake();
        // 设置全屏分辨率为 1920*1080
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow, 0);
    }
}
