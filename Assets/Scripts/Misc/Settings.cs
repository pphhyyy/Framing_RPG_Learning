using Unity.VisualScripting;
using UnityEngine;
public static class Settings 
{
    public const string PersistentScene = "PersistentScene";

    //控制玩家在场景物件背后，场景物件变成透明时的淡入淡出时间
    public const float fadeInSeconds = 0.25f;
    public const float fadeOutSeconds = 0.35f;
    public const float targetAlpha = 0.45f; //遮挡物体的透明值

    // Tilemap
    public const float gridCellSize = 1f;
    public static Vector2 cursorSize = Vector2.one;

    public static float playerCenterYOffset = 0.875f;

    public const float runningSpeed = 5.33f;
    public const float walkingSpeed = 2.66f;
    public static float useToolAnimationPause = 0.25f;
    public static float afterUseToolAnimationPause = 0.2f;

    public static float liftToolAnimationPause = 0.4f;
    public static float afterLiftToolAnimationPause = 0.4f;

    public static float pickAnimationPause = 1f;
    public static float afterpickAnimationPause = 0.2f;

    //Inventory 
    public static int playerInitialInventoryCapacity = 24;
    public static int playerMaximumInventoryCapacity = 48;

    public static int _xInput;
    public static int _yInput;
    public static int isWalking;
    public static int isRunning;
    public static int toolEffect;
    public static int isUsingToolRight;
    public static int isUsingToolLeft;
    public static int isUsingToolUp;
    public static int isUsingToolDown;
    public static int isLiftingToolRight;
    public static int isLiftingToolLeft;
    public static int isLiftingToolUp;
    public static int isLiftingToolDown;
    public static int isSwingingToolRight;
    public static int isSwingingToolLeft;
    public static int isSwingingToolUp;
    public static int isSwingingToolDown;
    public static int isPickingToolRight;
    public static int isPickingToolLeft;
    public static int isPickingToolUp;
    public static int isPickingToolDown;

    public static int idleRight;
    public static int idleLeft;
    public static int idleUp;
    public static int idleDown;

    //Tool 
    public const string HoeingTool = "Hoe";
    public const string ChoppingTool = "Axe";
    public const string BreakingTool = "Pickaxe";
    public const string ReapingTool = "Scythe";
    public const string WateringTool = "Watering Can";
    public const string CollectingTool = "Basket";

    //Reaping
    public const int maxCollidersToTestPerReapSwing = 15;  // 玩家挥动镰刀时候检测范围内 可以收割（只检测不实际收割） 的 item 的最大数目
    public const int maxTargetComponentsToDestroyPerReapSwing = 2; // 玩家挥动镰刀时候检测范围内 收割（确实的 destroy） 的 item 的最大数目

    //时间系统
    public const float secondsPerGameSecond = 0.012f;


    //静态的构造函数
    static Settings()
    {
        _xInput = Animator.StringToHash("xInput");
        _yInput = Animator.StringToHash("yInput");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");
        toolEffect = Animator.StringToHash("toolEffect");
        isUsingToolRight = Animator.StringToHash("isUsingToolRight");
        isUsingToolLeft = Animator.StringToHash("isUsingToolLeft");
        isUsingToolUp = Animator.StringToHash("isUsingToolUp");
        isUsingToolDown = Animator.StringToHash("isUsingToolDown");
        isLiftingToolRight = Animator.StringToHash("isLiftingToolRight");
        isLiftingToolLeft = Animator.StringToHash("isLiftingToolLeft");
        isLiftingToolUp = Animator.StringToHash("isLiftingToolUp");
        isLiftingToolDown = Animator.StringToHash("isLiftingToolDown");
        isSwingingToolRight = Animator.StringToHash ("isSwingingToolRight");
        isSwingingToolLeft = Animator.StringToHash  ("isSwingingToolLeft");
        isSwingingToolUp = Animator.StringToHash    ("isSwingingToolUp");
        isSwingingToolDown = Animator.StringToHash  ("isSwingingToolDown");

        isPickingToolRight = Animator.StringToHash  ("isPickingRight");
        isPickingToolLeft = Animator.StringToHash   ("isPickingLeft");
        isPickingToolUp = Animator.StringToHash     ("isPickingUp");
        isPickingToolDown = Animator.StringToHash   ("isPickingDown");

        idleRight = Animator.StringToHash   ("idleRight");
        idleLeft = Animator.StringToHash    ("idleLeft");
        idleUp = Animator.StringToHash      ("idleUp");
        idleDown = Animator.StringToHash    ("idleDown");
    }
}
