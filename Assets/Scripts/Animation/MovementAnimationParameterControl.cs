using UnityEngine;

public class MovementAnimationParameterControl : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {//添加消息订阅
        EventHandler.MovementEvent += SetAnimationParameters;
    }
    private void OnDisable()
    {//取消消息订阅
        EventHandler.MovementEvent += SetAnimationParameters;
    }

    private void SetAnimationParameters(float inputX, float inputY, ToolEffect tooleffect, bool isCarrying,
    bool isWalking, bool isRunning, bool isIdleing,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool isPickingToolRight, bool isPickingToolLeft, bool isPickingToolUp, bool isPickingToolDown,
    bool idleRight, bool idleLeft, bool idleUp, bool idleDown)
    {
        animator.SetFloat(Settings._xInput, inputX);
        animator.SetFloat (Settings._yInput, inputY);
        animator.SetBool(Settings.isWalking, isWalking);
        animator.SetBool(Settings.isRunning, isRunning);

        animator.SetInteger(Settings.toolEffect, (int)tooleffect);

        if(isWalking) 
            animator.SetTrigger(Settings.isWalking);
        if(isRunning) 
            animator.SetTrigger(Settings.isRunning);

        if (isUsingToolDown)
            animator.SetTrigger(Settings.isUsingToolDown);
        if (isUsingToolLeft)
            animator.SetTrigger(Settings.isUsingToolLeft);
        if (isUsingToolRight)
            animator.SetTrigger(Settings.isUsingToolRight);
        if (isUsingToolUp)
            animator.SetTrigger(Settings.isUsingToolUp);


        if (isLiftingToolRight)
            animator.SetTrigger(Settings.isLiftingToolRight);
        if (isLiftingToolLeft)
            animator.SetTrigger(Settings.isLiftingToolLeft);
        if (isLiftingToolUp)
            animator.SetTrigger(Settings.isLiftingToolUp);
        if (isLiftingToolDown)
            animator.SetTrigger(Settings.isLiftingToolDown);

        if (isSwingingToolRight)
            animator.SetTrigger(Settings.isSwingingToolRight);
        if (isSwingingToolLeft)
            animator.SetTrigger(Settings.isSwingingToolLeft);
        if (isSwingingToolUp)
            animator.SetTrigger(Settings.isSwingingToolUp);
        if (isSwingingToolDown)
            animator.SetTrigger(Settings.isSwingingToolDown);

        if (isPickingToolRight)
            animator.SetTrigger(Settings.isPickingToolRight);
        if (isPickingToolLeft)
            animator.SetTrigger(Settings.isPickingToolLeft);
        if (isPickingToolUp)
            animator.SetTrigger(Settings.isPickingToolUp);
        if (isPickingToolDown)
            animator.SetTrigger(Settings.isPickingToolDown);

        if (idleUp)
            animator.SetTrigger(Settings.idleUp);
        if (idleDown)
            animator.SetTrigger(Settings.idleDown);
        if (idleRight)
            animator.SetTrigger(Settings.idleRight);
        if (idleLeft)
            animator.SetTrigger(Settings.idleLeft);
    }
    private void AnimationEventPlayFootstepSound()
    {

    }
}
