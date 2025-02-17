using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager> , ISaveable 
{

    // git 更改测试 
    //定义各种时间参数
    private int gameYear = 1;
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

    private void OnEnable()
    {
        ISaveable_Register();

        //注册场景切换后要执行的事�? 

        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        
        EventHandler.AfterSceneloadEvent += AfterSceneLoadFadeIn;
    }


    void OnDisable()
    {
        ISaveable_Deregister();

        //注册场景切换后要执行的事�? 

        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        
        EventHandler.AfterSceneloadEvent -= AfterSceneLoadFadeIn;   
    }

    private void AfterSceneLoadFadeIn()
    {
        gameClockPaused = false;
    }

    private void BeforeSceneUnloadFadeOut()
    {
        gameClockPaused = true;
    }

    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek , gameHour, gameMinute, gameSecond);
    }

    private void Update()
    {
        if(!gameClockPaused)
        {
            GameTick(); // 开始转动计�?
        }
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime;

        if(gameTick >= Settings.secondsPerGameSecond) // 大于一秒，就让秒针开始转�?
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
                        //这里season 是用枚举来记录的，游戏里面一个月就直接换季节�? �? 然后换季节的时候用中间            变量 来增加季�?
                        int gs = (int)gameSeason;
                        gs++;
                        gameSeason = (Season)gs;

                        if(gs > 3)
                        {
                            gs = 0;
                            gameSeason = (Season)gs;
                            gameYear++;

                            if (gameYear > 9999)
                                gameYear = 1;

                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                        }
                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                    }
                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                }

                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            }

            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

           // Debug.Log("�?:" + gameYear + " 季节 :" + gameSeason + "  �? :" + gameDay + " 小时 :" + gameHour + " �?:" + gameMinute + "  �?:" + gameSecond);
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

    

     public void ISaveable_Register()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveable_Deregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {

        GameObjectSave.sceneData.Remove(Settings.PersistentScene);
        SceneSave sceneSave = new SceneSave();

        sceneSave.intDictionary = new Dictionary<string, int>();

        sceneSave.stringDictionary = new Dictionary<string, string>();

        //�? int 字典中添加数�?
        sceneSave.intDictionary.Add("gameYear",gameYear);
        sceneSave.intDictionary.Add("gameDay",gameDay);
        sceneSave.intDictionary.Add("gameHour",gameHour);
        sceneSave.intDictionary.Add("gameMinute",gameMinute);
        sceneSave.intDictionary.Add("gameSecond",gameSecond);

        //向string 数组中添加数�?
        sceneSave.stringDictionary.Add("gameDayOfWeek",gameDayOfWeek);
        sceneSave.stringDictionary.Add("gameSeason",gameSeason.ToString());

        GameObjectSave.sceneData.Add(Settings.PersistentScene,sceneSave);

        return GameObjectSave;


    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID,out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // 需要查找存货列�?,试着定位 这个游戏对象�? savescene 
            if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene,out SceneSave sceneSave))
            {

                //是否能找�? int �? string 字典 
                if(sceneSave.intDictionary != null && sceneSave.stringDictionary != null)
                {
                    //填充(populate) int �? save 数据
                    if(sceneSave.intDictionary.TryGetValue("gameYear",out int savedGameYear))
                        gameYear = savedGameYear;

                    if(sceneSave.intDictionary.TryGetValue("gameDay",out int savedGameDay))
                        gameDay = savedGameDay;

                    if(sceneSave.intDictionary.TryGetValue("gameHour",out int savedGameHour))
                        gameHour = savedGameHour;

                    if(sceneSave.intDictionary.TryGetValue("gameMinute",out int savedGameMinute))
                        gameMinute = savedGameMinute;

                    if(sceneSave.intDictionary.TryGetValue("gameSecond",out int savedSecond))
                        gameSecond = savedSecond;

                    if(sceneSave.stringDictionary.TryGetValue("gameDayOfWeek",out string savedDayOfWeek))
                        gameDayOfWeek = savedDayOfWeek;


                    if (sceneSave.stringDictionary.TryGetValue("gameSeason",out string savedGameSeason))
                    {
                        if(Enum.TryParse<Season>(savedGameSeason,out Season season))
                        {
                            gameSeason = season;
                        }
                    }

                    //时间归零
                    gameTick = 0f;

                    //触发时间更新
                    EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

                    //更新 游戏时钟
                }
            }
        }
    }
    public void ISaveable_StoreScene(string sceneName)
    {
        // 什么也不用�? 
    }

    public void ISaveable_RestoreScene(string sceneName)
    {
        // 什么也不用�? 
    }
}
