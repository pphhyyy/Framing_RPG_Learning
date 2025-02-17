using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerAnimationTest : MonoBehaviour
{
    public float _xInput;
    public float _yInput;
    public bool isWalking;
    public bool isRunning;
    public bool isIdle;
    public bool isCarrying;
    public ToolEffect toolEffect;
    public bool isUsingToolRight;
    public bool isUsingToolLeft;
    public bool isUsingToolUp;
    public bool isUsingToolDown;
    public bool isLiftingToolRight;
    public bool isLiftingToolLeft;
    public bool isLiftingToolUp;
    public bool isLiftingToolDown;
    public bool isSwingingToolRight;
    public bool isSwingingToolLeft;
    public bool isSwingingToolUp;
    public bool isSwingingToolDown;
    public bool isPickingToolRight;
    public bool isPickingToolLeft;
    public bool isPickingToolUp;
    public bool isPickingToolDown;
    public bool idleRight;
    public bool idleLeft;
    public bool idleUp;
    public bool idleDown;

    private void Update()
    {
        EventHandler.CallMovementEvent(_xInput, _yInput, toolEffect, isCarrying,
        isWalking, isRunning, isIdle,
     isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
     isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
     isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
     isPickingToolRight, isPickingToolLeft, isPickingToolUp, isPickingToolDown,
     idleRight, idleLeft, idleUp, idleDown);
    }
}
