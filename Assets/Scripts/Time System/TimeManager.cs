using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>,ISaveable
{

    // git 更改测试 
    //定义各种时间参数
    private int gamerYear = 1;
    private Season gameSeason = Season.Spring;
    private int gameDay = 1;
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;
    private string gameDayOfWeek = "Mon";
    private bool gameClockPaused = false; //当前时间是否暂停
    private float gameTick = 0f;

     private string _iSaveableUniqueID;

    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set {  _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;

    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }



    protected override void Awake()
    {
        base.Awake();  

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        GameObjectSave = new GameObjectSave();
    }


    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(gamerYear, gameSeason, gameDay, gameDayOfWeek , gameHour, gameMinute, gameSecond);
    }

    private void Update()
    {
        if(!gameClockPaused)
        {
            GameTick(); // 开始转动计时
        }
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime;

        if(gameTick >= Settings.secondsPerGameSecond) // 大于一秒，就让秒针开始转动
        {
            gameTick -= Settings.secondsPerGameSecond;

            UpdateGameSecond();
        }
    }

    private void UpdateGameSecond()
    {
        gameSecond++;

        if(gameSecond > 59)
        {
            gameSecond = 0;
            gameMinute++;
            if(gameMinute > 59)
            {
                gameMinute = 0;
                gameHour++;
                if(gameHour > 23)
                {
                    gameHour = 0;
                    gameDay ++;
                    if(gameDay > 30)
                    {
                        gameDay = 1;
                        //这里season 是用枚举来记录的，游戏里面一个月就直接换季节了 ， 然后换季节的时候用中间 变量 来增加季节
                        int gs = (int)gameSeason;
                        gs++;
                        gameSeason = (Season)gs;

                        if(gs > 3)
                        {
                            gs = 0;
                            gameSeason = (Season)gs;
                            gamerYear++;

                            if (gamerYear > 9999)
                                gamerYear = 1;

                            EventHandler.CallAdvanceGameYearEvent(gamerYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                        }
                        EventHandler.CallAdvanceGameSeasonEvent(gamerYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                    }
                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gamerYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                }

                EventHandler.CallAdvanceGameHourEvent(gamerYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            }

            EventHandler.CallAdvanceGameMinuteEvent(gamerYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

           // Debug.Log("年:" + gamerYear + " 季节 :" + gameSeason + "  日 :" + gameDay + " 小时 :" + gameHour + " 分:" + gameMinute + "  秒:" + gameSecond);
        }
    }

    private string GetDayOfWeek()
    {   //计算当前是星期几
        int TotalDays = (((int)gameSeason) * 30) + gameDay ;
        int dayOfWeek = TotalDays % 7 + 1;

        switch (dayOfWeek)
        {
            case 1:
                return "Mon";
            case 2:
                return "Tue";
            case 3:
                return "Wed";
            case 4:
                return "Thu";
            case 5:
                return "Fri";
            case 6:
                return "Sat";
            case 7:
                return "Sun";
            default:
                return "";
        }
    }

    public void Test_AdvanceGameDay()
    {
        for(int i = 0; i< 60; i++)
        {
            UpdateGameSecond();
        }
    }

    public void Test_AdvanceGameMinute()
    {
        for (int i = 0; i < 86400; i++)
        {
            UpdateGameSecond();
        }
    }
}
