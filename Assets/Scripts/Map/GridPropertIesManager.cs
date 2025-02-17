using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertIesManager : SingletonMonobehaviour<GridPropertIesManager>, ISaveable
{
    private Transform cropParentTransform;
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;

    private Grid grid;
    
    private bool isFirstTimeSceneLoaded = true; //是否是第一次加载这个场景，如果是，就要用crop 的初始化加载器 instantiator 
    //为场景中 各种 crop 设置 初始的 属性 ，比如 种植过了多少天，之类的，下一次加载该场景时候就不能使用初始化了，否则之前种植的进度就清空了
    
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;

    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;
    [SerializeField] private SO_GridPropertise[]  so_gridPropertiesArry = null; // 从当前 场景对应 的 so 文件中获取
    [SerializeField] private Tile[] dugGround = null;
    [SerializeField] private Tile[] waterGround = null;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }
    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void AdvanceDay(int gamerYear, Season gameSeason, int gameDay, string gameDayOfWeak, int gamerHour, int gameMinue, int gameSecond)
    {
        //每经历一天 需要处理一些东西 （比如今天给土地浇的水第二天应该干了，也就是第二天不能显示 浇过水的土地 
        ClearDisplayGridPropertyDetails();

        foreach (SO_GridPropertise so_GridProperties in so_gridPropertiesArry)
        {
            if (GameObjectSave.sceneData.TryGetValue(so_GridProperties.sceneName.ToString(), out SceneSave sceneSave))
            {
  
                if (sceneSave.gridPropertyDetailsDictionary != null)
                {
                    for (int i = sceneSave.gridPropertyDetailsDictionary.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDictionary.ElementAt(i);

                        GridPropertyDetails gridPropertyDetails = item.Value;


                        //更新全部 grid 的属性 ,growthDays 加一天 
                        if (gridPropertyDetails.growthDays > -1)
                        {
                            gridPropertyDetails.growthDays += 1;
                        }


                        //更新全部 grid 的属性 来反应 这一天 已经 过去了 
                        if (gridPropertyDetails.daySinceWatered > -1)
                        {
                            gridPropertyDetails.daySinceWatered = -1;
                        }

                        SetGridPropertyDetials(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDictionary);

                    }
                }
            }
        }

        DisplayGridPropertyDetails();
    }

    private void OnEnable()
    {
        ISaveable_Register();
        EventHandler.AfterSceneloadEvent += AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }

    private void OnDisable()
    {
        ISaveable_Deregister();
        EventHandler.AfterSceneloadEvent -= AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }

    

    private void Start()
    {
        InitialiseGridProperties();
    }

   

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecoration();
        Debug.Log("清除所有 crop ");
        ClearDisplayAllPlantedCrops();
    }

    private void ClearDisplayGroundDecoration()
    {
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayAllPlantedCrops()
    {
        //清除场景上所有 的 crop 对象
        Crop[] cropArray;
        cropArray = FindObjectsOfType<Crop>();  

        foreach(Crop crop in cropArray)
        {
            Destroy(crop.gameObject);
        }
    }



    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if(gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGound(gridPropertyDetails); // 要连接，是因为 一个 方块被挖掉以后 要和周围的方块产生联动 改变周围方块上显示的 sprite 
        }
    }

    public void DisplayWaterGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daySinceWatered > -1)
        {
            ConnectWaterGound(gridPropertyDetails); // 要连接，是因为 一个 方块被挖掉以后 要和周围的方块产生联动 改变周围方块上显示的 sprite 
        }
    }

    public void DisplayPlantedCrops(GridPropertyDetails gridPropertyDetails)
    {
        if(gridPropertyDetails.seedItemCode > -1)  // 大于负一 说明 gridPropertyDetails 中 存在种子 
        {
            CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

            if(cropDetails != null)
            {           
                GameObject cropPrefab;

                int growthStages = cropDetails.growthDay.Length;

                int currentGrowthStage = 0;
                //这个循环用于计算当前生长的阶段
                for(int i =  growthStages -1; i >= 0; i--)
                {
                    if(gridPropertyDetails.growthDays >= cropDetails.growthDay[i])
                    {
                        currentGrowthStage = i;
                        break;
                    }
                }

                //根据上面计算得到的当前的生长阶段 设置对应的prefab中的位置和sprite 等参数 
                cropPrefab = cropDetails.growthPrefab[currentGrowthStage];

                Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];

                Vector3 worldPosition = groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));

                worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2 , worldPosition.y , worldPosition.z);

                GameObject cropInstance = Instantiate(cropPrefab , worldPosition , Quaternion.identity); 

                cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
                cropInstance.transform.SetParent(cropParentTransform);
                cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX , gridPropertyDetails.gridY);
            }
        }
    }

    private void DisplayGridPropertyDetails()
    {
        foreach (KeyValuePair<string, GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);

            DisplayWaterGround(gridPropertyDetails);

            DisplayPlantedCrops(gridPropertyDetails);
        }
    }

    

    private void ConnectWaterGound(GridPropertyDetails gridPropertyDetails)
    {
        //处理 当前 grid 上的 tile 
        Tile WaterTile0 = SetWaterTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY); // 获得对应位置上的 tile 
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), WaterTile0);


        //处理 附近 上下左右 四个 grid 上的 tiel 
        GridPropertyDetails adjacentGridPropertyDetails; // 附近 grid 上的 PropertyDetails

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceWatered > -1) //说明这个 adjacentGrid 也被挖过了
        {
            Tile WaterTile1 = SetWaterTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), WaterTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceWatered > -1) //说明这个 adjacentGrid 也被挖过了
        {
            Tile WaterTile2 = SetWaterTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), WaterTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceWatered > -1) //说明这个 adjacentGrid 也被挖过了
        {
            Tile WaterTile3 = SetWaterTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), WaterTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceWatered > -1) //说明这个 adjacentGrid 也被挖过了
        {
            Tile WaterTile4 = SetWaterTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), WaterTile4);
        }
    }

    

    private void ConnectDugGound(GridPropertyDetails gridPropertyDetails)
    {
        //处理 当前 grid 上的 tile 
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY); // 获得对应位置上的 tile 
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX , gridPropertyDetails.gridY , 0) , dugTile0 );


        //处理 附近 上下左右 四个 grid 上的 tiel 
        GridPropertyDetails adjacentGridPropertyDetails; // 附近 grid 上的 PropertyDetails

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1) //说明这个 adjacentGrid 也被挖过了
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1) //说明这个 adjacentGrid 也被挖过了
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1 , gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1) //说明这个 adjacentGrid 也被挖过了
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX - 1 , gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1 , gridPropertyDetails.gridY, 0), dugTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1) //说明这个 adjacentGrid 也被挖过了
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), dugTile4);
        }
    }

    private Tile SetWaterTile(int gridX, int gridY)
    {
        bool upDug = IsGridSquareWater(gridX, gridY + 1); // 看这个 位置的 grid 有没有 被 挖 过 
        bool downDug = IsGridSquareWater(gridX, gridY - 1);
        bool leftDug = IsGridSquareWater(gridX - 1, gridY);
        bool rightDug = IsGridSquareWater(gridX + 1, gridY);

        //print("upDug" + upDug.ToString() + "=== downDug" + downDug + "=== leftDug" + leftDug + "=== rightDug" + rightDug);
        print("upDug" +  upDug.ToString() + "=== downDug" + downDug + "=== leftDug" + leftDug + "=== rightDug" + rightDug);
        #region 根据上面查看的结果 对 四种bool 对应 十六种 可能性

        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return waterGround[0];
        }
        else if (!upDug && downDug && rightDug && !leftDug)
        {
            return waterGround[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return waterGround[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return waterGround[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return waterGround[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return waterGround[5];
        }
        else if (upDug && downDug && rightDug && leftDug)
        {
            return waterGround[6];
        }
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return waterGround[7];
        }
        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return waterGround[8];
        }
        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return waterGround[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return waterGround[10];
        }
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return waterGround[11];
        }
        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return waterGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return waterGround[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return waterGround[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return waterGround[15];
        }



        return null;

        #endregion
    }

   

    private Tile SetDugTile(int gridX, int gridY)
    {
        bool upDug = IsGridSquareDug(gridX, gridY + 1); // 看这个 位置的 grid 有没有 被 挖 过 
        bool downDug = IsGridSquareDug(gridX, gridY - 1);
        bool leftDug = IsGridSquareDug(gridX-1, gridY);
        bool rightDug = IsGridSquareDug(gridX+1, gridY);

        //print("upDug" +  upDug.ToString() + "=== downDug" + downDug + "=== leftDug" + leftDug + "=== rightDug" + rightDug);

        #region 根据上面查看的结果 对 四种bool 对应 十六种 可能性

        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[0];
        }
        else if (!upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return dugGround[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }
        else if (upDug && downDug && rightDug && leftDug)
        {
            return dugGround[6];
        }
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }
        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[8];
        }
        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[10];
        }
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[11];
        }
        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[15];
        }



        return null;

        #endregion
    }


    private bool IsGridSquareWater(int gridX, int gridY)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(gridX, gridY);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daySinceWatered > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsGridSquareDug(int gridX, int gridY)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(gridX, gridY);

        if(gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    

    private void InitialiseGridProperties()  // 初始化网格属性
    {
        foreach(SO_GridPropertise so_GridPropterties in so_gridPropertiesArry)
        {
            Dictionary<string,GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            foreach(GridProperty gridProperty in so_GridPropterties.gridPropertyList)
            {
                GridPropertyDetails gridpropertyDetails;
                gridpropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDictionary);

                if(gridpropertyDetails == null)
                {
                    gridpropertyDetails =new GridPropertyDetails();
                }

                switch (gridProperty.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                        gridpropertyDetails.isDiggable = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.canDropItem:
                        gridpropertyDetails.canDropItem = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.canPlaceFurniture:
                        gridpropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.isPath:
                        gridpropertyDetails.isPath = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.isNPCObstacle:
                        gridpropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                        break;
                    default:
                        break;
                }
                SetGridPropertyDetials(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridpropertyDetails, gridPropertyDictionary);
            }

            //将上面配置的各种属性 加载到  sceneSave 中，方便游戏保存当前的进度 
            SceneSave sceneSave = new SceneSave();

            sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

            if(so_GridPropterties.sceneName.ToString() == SceneControllerManager.Instance.StartingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }

            // 添加一个bool 字典，并设置当前场景的名字为 true，表示这个场景已经加载过的 
            sceneSave.boolDictionary = new Dictionary<string, bool>();
            sceneSave.boolDictionary.Add("isFirstTimeSceneLoad", true);
            //之后每次 保存和加载的时候都可以从这里看到 当前场景是否是第一次加载 

            print("Debug2" + so_GridPropterties.sceneName);
            
            //Debug.Log(so_GridPropterties.sceneName.ToString());
            GameObjectSave.sceneData.Add(so_GridPropterties.sceneName.ToString() , sceneSave);
        }

    }

    public void SetGridPropertyDetials(int x , int y , GridPropertyDetails gridpropertyDetails)
    {
        SetGridPropertyDetials(x,y,gridpropertyDetails , gridPropertyDictionary);
    }

    private void SetGridPropertyDetials(int x, int y, GridPropertyDetails gridpropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        string key = "x" + x + "y" + y;

        gridpropertyDetails.gridX = x;
        gridpropertyDetails.gridY = y;

        gridPropertyDictionary[key] = gridpropertyDetails;
    }

    public GridPropertyDetails GetGridPropertyDetails(int x , int y)
    {
        return GetGridPropertyDetails(x , y , gridPropertyDictionary);
    }

    private GridPropertyDetails GetGridPropertyDetails(int x, int y, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        string key = "x" + x + "y" + y;
        GridPropertyDetails gridpropertyDetails;

        if(!gridPropertyDictionary.TryGetValue(key, out gridpropertyDetails))
        {
            return null;
        }
        else
        {
            return gridpropertyDetails;
        }
    }


    public Crop GetCropObjectAtGridLocation(GridPropertyDetails gridpropertyDetails)
    {
        //grid 坐标 到 世界坐标的转换 
        Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridpropertyDetails.gridX, gridpropertyDetails.gridY, 0));
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(worldPosition);//根据转换得到的世界坐标，找到该坐标 “点” 上的 Collider2D 数组

        Crop crop = null;

        for (int i = 0; i < collider2DArray.Length; i++) //遍历 collider2DArray ，从父对象和子对象两个方向寻找挂在了 Crop 组件的对象。然后返回即可
        {
            crop = collider2DArray[i].gameObject.GetComponentInParent<Crop>();
            if(crop != null && crop.cropGridPosition == new Vector2Int(gridpropertyDetails.gridX , gridpropertyDetails.gridY))
                break;
            crop = collider2DArray[i].gameObject.GetComponentInChildren<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridpropertyDetails.gridX, gridpropertyDetails.gridY))
                break;
        }

        return crop;
    }

    public CropDetails GetCropDetails(int seedItemCode)
    {
        return so_CropDetailsList.GetCropDetails(seedItemCode);
    }
    private void AfterSceneLoaded()
    {

        if(GameObject.FindGameObjectWithTag(Tags.CropsParentTransform) != null)
        {
            cropParentTransform = GameObject.FindGameObjectWithTag(Tags.CropsParentTransform).transform;
        }

        else
        {
            cropParentTransform  = null;
        }    

        // 获取 grid
        grid = FindObjectOfType<Grid>();

        // 获取 tile map 
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
    }

    

    public void ISaveable_Deregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveable_Register()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }



    public GameObjectSave ISaveableSave()
    {
        ISaveable_StoreScene(SceneManager.GetActiveScene().name);
        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID,out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;
            ISaveable_RestoreScene(SceneManager.GetActiveScene().name);
        }
    }
    public void ISaveable_RestoreScene(string sceneName)
    {
        /* 错误日志
        这里传入的参数 sceneName 是由 SaveLoadManager 通过 SceneManager.GetActiveScene().name 得到的，也就是场景上活动的 sceneName
        而 GameObjectSave.sceneData 中 作为 key 的 sceneName ， 却是在 InitialiseGridProperties 中 通过 配置的 so_ 文件中 enum SceneName 活得
        之前出现的 切换场景后 新场景中找不到可以放置物体的 grid 的bug ，就是 因为 在 enum SceneName 中 SceneName 首字母大写，与 实际的 sceneName 不同 
        导致 这里的 TryGetValue 无法找到对应场景的 sceneSave ，无法在切换场景时， 获取新场景中的 sceneSave.gridPropertyDetailsDictionary 
         GetGridPropertyDetails 中 gridPropertyDetailsDictionary.TryGetValue 就无法得到对应网格上的网格是否是可放置的（因为压根没有这个东西）

        第一次进入的场景之所以能用，是因为 gridPropertyDetailsDictionary.TryGetValue 只是查找 当前场景的 gridPropertyDetailsDictionary
        即使现在的 GameObjectSave.sceneData 中 sceneName 和 需要的不一样，也不影响第一次装入的内容
        */
        //Debug.Log("是否是第一次加载" + sceneName);

        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if(sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }
           // Debug.Log("是否是第一次加载");
            //从 sceneSave 的 boolDictionary 中 尝试获得 当前场景是否是第一次加载，并用try 得到的值 修改 manager 中 的 isFirstTimeSceneLoaded
            if (sceneSave.boolDictionary != null && sceneSave.boolDictionary.TryGetValue("isFirstTimeSceneLoad",out bool storedIsFirstTimeSceneLoaded))
            {
                //Debug.Log("第一次加载");
                isFirstTimeSceneLoaded = storedIsFirstTimeSceneLoaded;
            }

            if (isFirstTimeSceneLoaded) //如果是第一次加载当前场景，就要调用eventhandler 的 方法，来初始化 各种 crop 的 prefab
                EventHandler.CallInstantiateCropPrefabsEvent(); //实际调用的是 CropInstantiator 中的 InstantiateCropPrefabs ，
                //场景上的 crop (就是那些一开始就存在的crop 树苗 萝卜苗 之类的，都会挂载一个 CropInstantiator ， 并订阅 CallInstantiateCropPrefabsEvent
                //然后这边manager 只要 CallInstantiateCropPrefabsEvent ， 就可以让场景上的crop 都调用他们自己挂载的 CropInstantiator 中 的 InstantiateCropPrefabs

            //如果当前场景存在  gridPropertyDictionary ，清理原来的  GridPropertyDetails ， DisplayGridPropertyDetails 显示  GridPropertyDetails
            if (gridPropertyDictionary.Count > 0)
            {
                ClearDisplayGridPropertyDetails();

                DisplayGridPropertyDetails();
            }

            if (isFirstTimeSceneLoaded == true)
            { 
                //只要 初始化一次 后面就不应该初始化了 
                isFirstTimeSceneLoaded = false;
            }
        }
    }

    public void ISaveable_StoreScene(string sceneName)
    {
        //保存当前场景 的 grid 信息 
        GameObjectSave.sceneData.Remove(sceneName);
        SceneSave sceneSave = new SceneSave();

        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;
        
        //保存的时候要把当前场景是否是第一次加载的信息保存进去 
        sceneSave.boolDictionary = new Dictionary<string, bool>();
        sceneSave.boolDictionary.Add("isFirstTimeSceneLoad", isFirstTimeSceneLoaded);
       

        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }


    

}
