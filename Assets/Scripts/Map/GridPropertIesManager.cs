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
    
    private bool isFirstTimeSceneLoaded = true; //�Ƿ��ǵ�һ�μ����������������ǣ���Ҫ��crop �ĳ�ʼ�������� instantiator 
    //Ϊ������ ���� crop ���� ��ʼ�� ���� ������ ��ֲ���˶����죬֮��ģ���һ�μ��ظó���ʱ��Ͳ���ʹ�ó�ʼ���ˣ�����֮ǰ��ֲ�Ľ��Ⱦ������
    
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;

    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;
    [SerializeField] private SO_GridPropertise[]  so_gridPropertiesArry = null; // �ӵ�ǰ ������Ӧ �� so �ļ��л�ȡ
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
        //ÿ����һ�� ��Ҫ����һЩ���� �������������ؽ���ˮ�ڶ���Ӧ�ø��ˣ�Ҳ���ǵڶ��첻����ʾ ����ˮ������ 
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


                        //����ȫ�� grid ������ ,growthDays ��һ�� 
                        if (gridPropertyDetails.growthDays > -1)
                        {
                            gridPropertyDetails.growthDays += 1;
                        }


                        //����ȫ�� grid ������ ����Ӧ ��һ�� �Ѿ� ��ȥ�� 
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
        Debug.Log("������� crop ");
        ClearDisplayAllPlantedCrops();
    }

    private void ClearDisplayGroundDecoration()
    {
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayAllPlantedCrops()
    {
        //������������� �� crop ����
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
            ConnectDugGound(gridPropertyDetails); // Ҫ���ӣ�����Ϊ һ�� ���鱻�ڵ��Ժ� Ҫ����Χ�ķ���������� �ı���Χ��������ʾ�� sprite 
        }
    }

    public void DisplayWaterGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daySinceWatered > -1)
        {
            ConnectWaterGound(gridPropertyDetails); // Ҫ���ӣ�����Ϊ һ�� ���鱻�ڵ��Ժ� Ҫ����Χ�ķ���������� �ı���Χ��������ʾ�� sprite 
        }
    }

    public void DisplayPlantedCrops(GridPropertyDetails gridPropertyDetails)
    {
        if(gridPropertyDetails.seedItemCode > -1)  // ���ڸ�һ ˵�� gridPropertyDetails �� �������� 
        {
            CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

            if(cropDetails != null)
            {           
                GameObject cropPrefab;

                int growthStages = cropDetails.growthDay.Length;

                int currentGrowthStage = 0;
                //���ѭ�����ڼ��㵱ǰ�����Ľ׶�
                for(int i =  growthStages -1; i >= 0; i--)
                {
                    if(gridPropertyDetails.growthDays >= cropDetails.growthDay[i])
                    {
                        currentGrowthStage = i;
                        break;
                    }
                }

                //�����������õ��ĵ�ǰ�������׶� ���ö�Ӧ��prefab�е�λ�ú�sprite �Ȳ��� 
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
        //���� ��ǰ grid �ϵ� tile 
        Tile WaterTile0 = SetWaterTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY); // ��ö�Ӧλ���ϵ� tile 
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), WaterTile0);


        //���� ���� �������� �ĸ� grid �ϵ� tiel 
        GridPropertyDetails adjacentGridPropertyDetails; // ���� grid �ϵ� PropertyDetails

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceWatered > -1) //˵����� adjacentGrid Ҳ���ڹ���
        {
            Tile WaterTile1 = SetWaterTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), WaterTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceWatered > -1) //˵����� adjacentGrid Ҳ���ڹ���
        {
            Tile WaterTile2 = SetWaterTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), WaterTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceWatered > -1) //˵����� adjacentGrid Ҳ���ڹ���
        {
            Tile WaterTile3 = SetWaterTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), WaterTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceWatered > -1) //˵����� adjacentGrid Ҳ���ڹ���
        {
            Tile WaterTile4 = SetWaterTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), WaterTile4);
        }
    }

    

    private void ConnectDugGound(GridPropertyDetails gridPropertyDetails)
    {
        //���� ��ǰ grid �ϵ� tile 
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY); // ��ö�Ӧλ���ϵ� tile 
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX , gridPropertyDetails.gridY , 0) , dugTile0 );


        //���� ���� �������� �ĸ� grid �ϵ� tiel 
        GridPropertyDetails adjacentGridPropertyDetails; // ���� grid �ϵ� PropertyDetails

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1) //˵����� adjacentGrid Ҳ���ڹ���
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1) //˵����� adjacentGrid Ҳ���ڹ���
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1 , gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1) //˵����� adjacentGrid Ҳ���ڹ���
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX - 1 , gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1 , gridPropertyDetails.gridY, 0), dugTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1) //˵����� adjacentGrid Ҳ���ڹ���
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), dugTile4);
        }
    }

    private Tile SetWaterTile(int gridX, int gridY)
    {
        bool upDug = IsGridSquareWater(gridX, gridY + 1); // ����� λ�õ� grid ��û�� �� �� �� 
        bool downDug = IsGridSquareWater(gridX, gridY - 1);
        bool leftDug = IsGridSquareWater(gridX - 1, gridY);
        bool rightDug = IsGridSquareWater(gridX + 1, gridY);

        //print("upDug" + upDug.ToString() + "=== downDug" + downDug + "=== leftDug" + leftDug + "=== rightDug" + rightDug);
        print("upDug" +  upDug.ToString() + "=== downDug" + downDug + "=== leftDug" + leftDug + "=== rightDug" + rightDug);
        #region ��������鿴�Ľ�� �� ����bool ��Ӧ ʮ���� ������

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
        bool upDug = IsGridSquareDug(gridX, gridY + 1); // ����� λ�õ� grid ��û�� �� �� �� 
        bool downDug = IsGridSquareDug(gridX, gridY - 1);
        bool leftDug = IsGridSquareDug(gridX-1, gridY);
        bool rightDug = IsGridSquareDug(gridX+1, gridY);

        //print("upDug" +  upDug.ToString() + "=== downDug" + downDug + "=== leftDug" + leftDug + "=== rightDug" + rightDug);

        #region ��������鿴�Ľ�� �� ����bool ��Ӧ ʮ���� ������

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

    

    private void InitialiseGridProperties()  // ��ʼ����������
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

            //���������õĸ������� ���ص�  sceneSave �У�������Ϸ���浱ǰ�Ľ��� 
            SceneSave sceneSave = new SceneSave();

            sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

            if(so_GridPropterties.sceneName.ToString() == SceneControllerManager.Instance.StartingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }

            // ���һ��bool �ֵ䣬�����õ�ǰ����������Ϊ true����ʾ��������Ѿ����ع��� 
            sceneSave.boolDictionary = new Dictionary<string, bool>();
            sceneSave.boolDictionary.Add("isFirstTimeSceneLoad", true);
            //֮��ÿ�� ����ͼ��ص�ʱ�򶼿��Դ����￴�� ��ǰ�����Ƿ��ǵ�һ�μ��� 

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
        //grid ���� �� ���������ת�� 
        Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridpropertyDetails.gridX, gridpropertyDetails.gridY, 0));
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(worldPosition);//����ת���õ����������꣬�ҵ������� ���㡱 �ϵ� Collider2D ����

        Crop crop = null;

        for (int i = 0; i < collider2DArray.Length; i++) //���� collider2DArray ���Ӹ�������Ӷ�����������Ѱ�ҹ����� Crop ����Ķ���Ȼ�󷵻ؼ���
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

        // ��ȡ grid
        grid = FindObjectOfType<Grid>();

        // ��ȡ tile map 
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
        /* ������־
        ���ﴫ��Ĳ��� sceneName ���� SaveLoadManager ͨ�� SceneManager.GetActiveScene().name �õ��ģ�Ҳ���ǳ����ϻ�� sceneName
        �� GameObjectSave.sceneData �� ��Ϊ key �� sceneName �� ȴ���� InitialiseGridProperties �� ͨ�� ���õ� so_ �ļ��� enum SceneName ���
        ֮ǰ���ֵ� �л������� �³������Ҳ������Է�������� grid ��bug ������ ��Ϊ �� enum SceneName �� SceneName ����ĸ��д���� ʵ�ʵ� sceneName ��ͬ 
        ���� ����� TryGetValue �޷��ҵ���Ӧ������ sceneSave ���޷����л�����ʱ�� ��ȡ�³����е� sceneSave.gridPropertyDetailsDictionary 
         GetGridPropertyDetails �� gridPropertyDetailsDictionary.TryGetValue ���޷��õ���Ӧ�����ϵ������Ƿ��ǿɷ��õģ���Ϊѹ��û�����������

        ��һ�ν���ĳ���֮�������ã�����Ϊ gridPropertyDetailsDictionary.TryGetValue ֻ�ǲ��� ��ǰ������ gridPropertyDetailsDictionary
        ��ʹ���ڵ� GameObjectSave.sceneData �� sceneName �� ��Ҫ�Ĳ�һ����Ҳ��Ӱ���һ��װ�������
        */
        //Debug.Log("�Ƿ��ǵ�һ�μ���" + sceneName);

        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if(sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }
           // Debug.Log("�Ƿ��ǵ�һ�μ���");
            //�� sceneSave �� boolDictionary �� ���Ի�� ��ǰ�����Ƿ��ǵ�һ�μ��أ�����try �õ���ֵ �޸� manager �� �� isFirstTimeSceneLoaded
            if (sceneSave.boolDictionary != null && sceneSave.boolDictionary.TryGetValue("isFirstTimeSceneLoad",out bool storedIsFirstTimeSceneLoaded))
            {
                //Debug.Log("��һ�μ���");
                isFirstTimeSceneLoaded = storedIsFirstTimeSceneLoaded;
            }

            if (isFirstTimeSceneLoaded) //����ǵ�һ�μ��ص�ǰ��������Ҫ����eventhandler �� ����������ʼ�� ���� crop �� prefab
                EventHandler.CallInstantiateCropPrefabsEvent(); //ʵ�ʵ��õ��� CropInstantiator �е� InstantiateCropPrefabs ��
                //�����ϵ� crop (������Щһ��ʼ�ʹ��ڵ�crop ���� �ܲ��� ֮��ģ��������һ�� CropInstantiator �� ������ CallInstantiateCropPrefabsEvent
                //Ȼ�����manager ֻҪ CallInstantiateCropPrefabsEvent �� �Ϳ����ó����ϵ�crop �����������Լ����ص� CropInstantiator �� �� InstantiateCropPrefabs

            //�����ǰ��������  gridPropertyDictionary ������ԭ����  GridPropertyDetails �� DisplayGridPropertyDetails ��ʾ  GridPropertyDetails
            if (gridPropertyDictionary.Count > 0)
            {
                ClearDisplayGridPropertyDetails();

                DisplayGridPropertyDetails();
            }

            if (isFirstTimeSceneLoaded == true)
            { 
                //ֻҪ ��ʼ��һ�� ����Ͳ�Ӧ�ó�ʼ���� 
                isFirstTimeSceneLoaded = false;
            }
        }
    }

    public void ISaveable_StoreScene(string sceneName)
    {
        //���浱ǰ���� �� grid ��Ϣ 
        GameObjectSave.sceneData.Remove(sceneName);
        SceneSave sceneSave = new SceneSave();

        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;
        
        //�����ʱ��Ҫ�ѵ�ǰ�����Ƿ��ǵ�һ�μ��ص���Ϣ�����ȥ 
        sceneSave.boolDictionary = new Dictionary<string, bool>();
        sceneSave.boolDictionary.Add("isFirstTimeSceneLoad", isFirstTimeSceneLoaded);
       

        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }


    

}
