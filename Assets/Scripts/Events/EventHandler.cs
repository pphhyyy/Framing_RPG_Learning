using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void MovementDelegate(float inputX, float inputY, ToolEffect tooleffect, bool isCarrying,
    bool isWalking, bool isRunning, bool isIdleing,
    bool isUsingToolRight , bool isUsingToolLeft , bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight , bool isLiftingToolLeft , bool isLiftingToolUp ,  bool isLiftingToolDown,
    bool isSwingingToolRight , bool isSwingingToolLeft ,bool isSwingingToolUp , bool isSwingingToolDwon,
    bool isPickingToolRight ,bool isPickingToolLeft , bool isPickingToolUp , bool isPickingToolDwon ,
    bool idleRight , bool idleLeft , bool idleUp , bool idleDown) ; //自定义的委托delegate 类型，

public class EventHandler
{

    //这里定义了一系列委托 和调用这些委托的函数， 需要订阅这些委托的类 自己写订阅事件，然后需要调用的类调用这里的函数即可


    public static event Action DropSelectedItemEvent;

    public static void CallDropSelectedItemEvent()
    {
        if(DropSelectedItemEvent != null) 
            DropSelectedItemEvent();
    }

    //将选中的 item 从 inventory 中移除
    public static event Action RemoveSelectedItemFromInventoryEvent;

    public static void CallRemoveSelectedItemFromInventoryEvent()
    {
        if(RemoveSelectedItemFromInventoryEvent != null) 
            RemoveSelectedItemFromInventoryEvent();
    }



    public static event Action<Vector3, HarvestActionEffect> HarvestActionEffectEvent;

    public static void CallHarvestActionEffectEvent(Vector3 effectPosition , HarvestActionEffect harvestActionEffect)
    {
        if(HarvestActionEffectEvent != null)
            HarvestActionEffectEvent(effectPosition , harvestActionEffect);

    }

    //Inventory Updated Event
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent; //因为需要传入的参数很少，所以 使用系统自带的Action 

    public static void CallInventoryUpdatedEvent(InventoryLocation Inventorylocation, List<InventoryItem> inventoryList)
    {
        if(InventoryUpdatedEvent != null)
        {
            InventoryUpdatedEvent(Inventorylocation, inventoryList);
        }
    }

    //crop 预设体 实例化
    public static event Action InstantiateCropPrefabsEvent;
    public static void CallInstantiateCropPrefabsEvent()
    {
        if (InstantiateCropPrefabsEvent != null)
        {
            InstantiateCropPrefabsEvent();
        }
    }


    //Movement Event
    public static event MovementDelegate MovementEvent;

    public static void CallMovementEvent(float inputX, float inputY, ToolEffect toolEffect,  bool isCarrying,
    bool isWalking, bool isRunning, bool isIdleing,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDwon,
    bool isPickingToolRight, bool isPickingToolLeft, bool isPickingToolUp, bool isPickingToolDwon,
    bool idleRight, bool idleLeft, bool idleUp, bool idleDown)
        {
        if(MovementEvent != null)
        {
            MovementEvent(inputX, inputY, toolEffect, isCarrying,
                isWalking, isRunning, isIdleing,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDwon,
                isPickingToolRight, isPickingToolLeft, isPickingToolUp, isPickingToolDwon,
                idleRight, idleLeft, idleUp, idleDown);
        }
    }

    //时间系统事件 分
    public static event Action<int, Season,  int, string, int, int, int> AdvanceGameMinuteEvent;

    public static void CallAdvanceGameMinuteEvent(int gamerYear, Season gameSeason ,  int gameDay , string gameDayOfWeak, int gamerHour, int gameMinue , int gameSecond)
    {
        if(AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(gamerYear ,  gameSeason , gameDay , gameDayOfWeak, gamerHour, gameMinue , gameSecond);
        }
    }

    //时间系统事件 小时
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(int gamerYear, Season gameSeason, int gameDay, string gameDayOfWeak, int gamerHour, int gameMinue, int gameSecond)
    {
        if (AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent(gamerYear, gameSeason, gameDay, gameDayOfWeak, gamerHour, gameMinue, gameSecond);
        }
    }

    //时间系统事件 天
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;

    public static void CallAdvanceGameDayEvent(int gamerYear, Season gameSeason, int gameDay, string gameDayOfWeak, int gamerHour, int gameMinue, int gameSecond)
    {
        if (AdvanceGameDayEvent != null)
        {
            AdvanceGameDayEvent(gamerYear, gameSeason, gameDay, gameDayOfWeak, gamerHour, gameMinue, gameSecond);
        }
    }

    //时间系统事件 季节
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;

    public static void CallAdvanceGameSeasonEvent(int gamerYear, Season gameSeason, int gameDay, string gameDayOfWeak, int gamerHour, int gameMinue, int gameSecond)
    {
        if (AdvanceGameSeasonEvent != null)
        {
            AdvanceGameSeasonEvent(gamerYear, gameSeason, gameDay, gameDayOfWeak, gamerHour, gameMinue, gameSecond);
        }
    }

    //时间系统事件 季节
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

    public static void CallAdvanceGameYearEvent(int gamerYear, Season gameSeason, int gameDay, string gameDayOfWeak, int gamerHour, int gameMinue, int gameSecond)
    {
        if (AdvanceGameYearEvent != null)
        {
            AdvanceGameYearEvent(gamerYear, gameSeason, gameDay, gameDayOfWeak, gamerHour, gameMinue, gameSecond);
        }
    }

    //当前场景unload 前 的淡出 事件
    public static event Action BeforeSceneUnloadFadeOutEvent;

    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if(BeforeSceneUnloadFadeOutEvent != null)
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }

    //当前场景 UnLoad 前 调用的事件
    public static event Action BeforeSceneUnloadEvent;

    public static void CallBeforeSceneUnloadEvent()
    {
        if (BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }

    //新场景 Load 后 调用的事件
    public static event Action AfterSceneloadEvent;

    public static void CallAfterSceneloadEvent()
    {
        if (AfterSceneloadEvent != null)
        {
            AfterSceneloadEvent();
        }
    }

    //当前场景 Load 后 淡入的事件
    public static event Action AfterSceneloadFadeInEvent;

    public static void CallAfterSceneloadFadeInEvent()
    {
        if (AfterSceneloadFadeInEvent != null)
        {
            AfterSceneloadFadeInEvent();
        }
    }
}
