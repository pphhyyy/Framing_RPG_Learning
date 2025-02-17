using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>,ISaveable
{

    // git ���Ĳ��� 
    //�������ʱ�����
    private int gamerYear = 1;
    private Season gameSeason = Season.Spring;
    private int gameDay = 1;
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;
    private string gameDayOfWeek = "Mon";
    private bool gameClockPaused = false; //��ǰʱ���Ƿ���ͣ
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
            GameTick(); // ��ʼת����ʱ
        }
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime;

        if(gameTick >= Settings.secondsPerGameSecond) // ����һ�룬�������뿪ʼת��
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
                        //����season ����ö������¼�ģ���Ϸ����һ���¾�ֱ�ӻ������� �� Ȼ�󻻼��ڵ�ʱ�����м� ���� �����Ӽ���
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

           // Debug.Log("��:" + gamerYear + " ���� :" + gameSeason + "  �� :" + gameDay + " Сʱ :" + gameHour + " ��:" + gameMinute + "  ��:" + gameSecond);
        }
    }

    private string GetDayOfWeek()
    {   //���㵱ǰ�����ڼ�
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
