using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void MovementDelegate(float inputX, float inputY, ToolEffect tooleffect, bool isCarrying,
    bool isWalking, bool isRunning, bool isIdleing,
    bool isUsingToolRight , bool isUsingToolLeft , bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight , bool isLiftingToolLeft , bool isLiftingToolUp ,  bool isLiftingToolDown,
    bool isSwingingToolRight , bool isSwingingToolLeft ,bool isSwingingToolUp , bool isSwingingToolDwon,
    bool isPickingToolRight ,bool isPickingToolLeft , bool isPickingToolUp , bool isPickingToolDwon ,
    bool idleRight , bool idleLeft , bool idleUp , bool idleDown) ; //�Զ����ί��delegate ���ͣ�

public class EventHandler
{

    //���ﶨ����һϵ��ί�� �͵�����Щί�еĺ����� ��Ҫ������Щί�е��� �Լ�д�����¼���Ȼ����Ҫ���õ����������ĺ�������


    public static event Action DropSelectedItemEvent;

    public static void CallDropSelectedItemEvent()
    {
        if(DropSelectedItemEvent != null) 
            DropSelectedItemEvent();
    }

    //��ѡ�е� item �� inventory ���Ƴ�
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
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent; //��Ϊ��Ҫ����Ĳ������٣����� ʹ��ϵͳ�Դ���Action 

    public static void CallInventoryUpdatedEvent(InventoryLocation Inventorylocation, List<InventoryItem> inventoryList)
    {
        if(InventoryUpdatedEvent != null)
        {
            InventoryUpdatedEvent(Inventorylocation, inventoryList);
        }
    }

    //crop Ԥ���� ʵ����
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

    //ʱ��ϵͳ�¼� ��
    public static event Action<int, Season,  int, string, int, int, int> AdvanceGameMinuteEvent;

    public static void CallAdvanceGameMinuteEvent(int gamerYear, Season gameSeason ,  int gameDay , string gameDayOfWeak, int gamerHour, int gameMinue , int gameSecond)
    {
        if(AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(gamerYear ,  gameSeason , gameDay , gameDayOfWeak, gamerHour, gameMinue , gameSecond);
        }
    }

    //ʱ��ϵͳ�¼� Сʱ
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(int gamerYear, Season gameSeason, int gameDay, string gameDayOfWeak, int gamerHour, int gameMinue, int gameSecond)
    {
        if (AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent(gamerYear, gameSeason, gameDay, gameDayOfWeak, gamerHour, gameMinue, gameSecond);
        }
    }

    //ʱ��ϵͳ�¼� ��
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;

    public static void CallAdvanceGameDayEvent(int gamerYear, Season gameSeason, int gameDay, string gameDayOfWeak, int gamerHour, int gameMinue, int gameSecond)
    {
        if (AdvanceGameDayEvent != null)
        {
            AdvanceGameDayEvent(gamerYear, gameSeason, gameDay, gameDayOfWeak, gamerHour, gameMinue, gameSecond);
        }
    }

    //ʱ��ϵͳ�¼� ����
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;

    public static void CallAdvanceGameSeasonEvent(int gamerYear, Season gameSeason, int gameDay, string gameDayOfWeak, int gamerHour, int gameMinue, int gameSecond)
    {
        if (AdvanceGameSeasonEvent != null)
        {
            AdvanceGameSeasonEvent(gamerYear, gameSeason, gameDay, gameDayOfWeak, gamerHour, gameMinue, gameSecond);
        }
    }

    //ʱ��ϵͳ�¼� ����
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

    public static void CallAdvanceGameYearEvent(int gamerYear, Season gameSeason, int gameDay, string gameDayOfWeak, int gamerHour, int gameMinue, int gameSecond)
    {
        if (AdvanceGameYearEvent != null)
        {
            AdvanceGameYearEvent(gamerYear, gameSeason, gameDay, gameDayOfWeak, gamerHour, gameMinue, gameSecond);
        }
    }

    //��ǰ����unload ǰ �ĵ��� �¼�
    public static event Action BeforeSceneUnloadFadeOutEvent;

    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if(BeforeSceneUnloadFadeOutEvent != null)
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }

    //��ǰ���� UnLoad ǰ ���õ��¼�
    public static event Action BeforeSceneUnloadEvent;

    public static void CallBeforeSceneUnloadEvent()
    {
        if (BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }

    //�³��� Load �� ���õ��¼�
    public static event Action AfterSceneloadEvent;

    public static void CallAfterSceneloadEvent()
    {
        if (AfterSceneloadEvent != null)
        {
            AfterSceneloadEvent();
        }
    }

    //��ǰ���� Load �� ������¼�
    public static event Action AfterSceneloadFadeInEvent;

    public static void CallAfterSceneloadFadeInEvent()
    {
        if (AfterSceneloadFadeInEvent != null)
        {
            AfterSceneloadFadeInEvent();
        }
    }
}
