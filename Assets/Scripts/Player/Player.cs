using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class Player : SingletonMonobehaviour<Player>, ISaveable
{

    //WaitForSeconds ��ͣЭ�� һ��ʱ��
    private WaitForSeconds afterLiftToolAnimationPause;
    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds afterPickAnimationPause;
    private AnimationOverrides animationOverrides;
    private GridCursor gridCursor;
    private Cursor cursor;

    //movement Parameters
    private float _xInput;
    private float _yInput;
    private bool isWalking;
    private bool isRunning;
    private bool isIdle;
    private bool isCarrying = false;
    private ToolEffect toolEffect = ToolEffect.none;
    private bool isUsingToolRight;
    private bool isUsingToolLeft;
    private bool isUsingToolUp;
    private bool isUsingToolDown;
    private bool isLiftingToolRight;
    private bool isLiftingToolLeft;
    private bool isLiftingToolUp;
    private bool isLiftingToolDown;
    private bool isSwingingToolRight;
    private bool isSwingingToolLeft;
    private bool isSwingingToolUp;
    private bool isSwingingToolDown;
    private bool isPickingToolRight;
    private bool isPickingToolLeft;
    private bool isPickingToolUp;
    private bool isPickingToolDown;
    private bool idleRight;
    private bool idleLeft;
    private bool idleUp;
    private bool idleDown;

    private Camera mainCamera;

    private bool playerToolUseDisabled = false; 

    private Rigidbody2D rigidBody2D;

    private WaitForSeconds useToolAnimationPause;
    private WaitForSeconds liftToolAnimationPause;
    private WaitForSeconds PickAnimationPause;


    private Direction playerDirection;


    private List<CharacterAttribute> characterAttributeCustomisationList ;
    private float movementSpeed;

    [Tooltip("Should be populated in the prefab with the equipped item spirte renderer")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null; //�������ѡ����Ʒ���е�ĳ��item ����Ҫ���������Ǹ������ sprite 

    private CharacterAttribute armsCharacterAttribute;
    private CharacterAttribute toolCharacterAttribute;

    private bool _playerInputIsDisabled = false;
    public bool PlayerInputIsDisabled { get => _playerInputIsDisabled; set => _playerInputIsDisabled = value; }

    // Isaveable �ӿ�Ҫ�������
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }



    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        rigidBody2D = GetComponent<Rigidbody2D>();

        animationOverrides = GetComponentInChildren<AnimationOverrides>();

        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.arms , PartVariantColor.none , PartVarianType.none ); //�ֲ��Ľ�ɫ���ԣ��ͺ���Ķ��������й�
        toolCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.tool, PartVariantColor.none, PartVarianType.none); //�ֲ��Ľ�ɫ���ԣ��ͺ���Ķ��������й�


        characterAttributeCustomisationList = new List<CharacterAttribute>();

        //�ӽű��� unique id ��������,��ȡ��ǰ�ű���Unique ID

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
        
        mainCamera =  Camera.main; //�����ȡ��mian camera �����Ǳ�ǩΪ maincamera �� camera 
    }

    private void OnEnable()
    {
        //ע�� ���� 
        ISaveable_Register();

        //���ǳ�������ʱ Ӧ�ùر� ������ƶ� 
        EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneloadFadeInEvent += EnablePlayerInput;

    }

    
    private void OnDisable()
    {
        ISaveable_Deregister();
        EventHandler.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneloadFadeInEvent -= EnablePlayerInput;
    }

    

    public void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();
        cursor = FindObjectOfType<Cursor>();    


        //�� Settings �����úõ�ֵ ��� ����� ���� WaitForSeconds �� ����������ͣ ��Ϊ���Ż� ʹ�� ����ʱ ���� ����ʾ Ч�� 
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
        PickAnimationPause = new WaitForSeconds(Settings.pickAnimationPause);
        afterPickAnimationPause = new WaitForSeconds(Settings.afterpickAnimationPause);

    }

    private void Update()
    {

        #region Player Input

        if(!PlayerInputIsDisabled)
        {
            ResetAnimationTriggers();
            PlayerMovementInput();
            PlayerWalkInput();

            PlayerClickInput(); //���ǲ��϶���ֱ��ѡ��Ȼ���� �������¼�

            PlayerTestInput();

            Debug.Log("����mac air ��Զ�̵���");
            
            EventHandler.CallMovementEvent(_xInput, _yInput, toolEffect, isCarrying,
            isWalking, isRunning, isIdle,
         isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
         isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
         isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
         isPickingToolRight, isPickingToolLeft, isPickingToolUp, isPickingToolDown,
         false, false, false, false);
        }
       
        #endregion
    }

   

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        Vector2 move = new Vector2(_xInput*movementSpeed*Time.deltaTime , _yInput * movementSpeed*Time.deltaTime);

        rigidBody2D.MovePosition(rigidBody2D.position + move);
    }

    private void PlayerClickInput()
    {
        if(!playerToolUseDisabled)
        {
            if (Input.GetMouseButtonDown(0))
            {

                if (gridCursor.CursorIsEnable || cursor.CursorIsEnable)
                {
                    //��ȡ �� ��� �� ��� ��λ�ã� 
                    Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();
                    Vector3Int PlayerGridPosition = gridCursor.GetGridPositionForPlayer();
                    ProcessPlayerClickInput(cursorGridPosition ,  PlayerGridPosition);
                }
            }
        }
        
    }

    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition , Vector3Int PlayerGridPosition)
    {
        ResetMovement();

        // ��� Ӧ�� ���� ������ڵķ��� 
        Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPosition, PlayerGridPosition);
        GridPropertyDetails gridPropertyDetails = GridPropertIesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if(itemDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if(Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputSeed(gridPropertyDetails ,  itemDetails);
                    }
                    break;
                case ItemType.Commodity:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputCommodity(itemDetails);
                    }
                    break;
                case ItemType.Watering_tool:
                case ItemType.Breaking_tool:
                case ItemType.Hoeing_tool:
                case ItemType.Chopping_tool:
                case ItemType.Reaping_tool:
                case ItemType.Collecting_tool:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
                    break;
                case ItemType.none:
                    break;
                case ItemType.count:
                    break;

                default:
                    break;
            }
        }
    }

    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        switch(itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if(gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails , playerDirection);
                }
                break;
            case ItemType.Watering_tool:
                if(gridCursor.CursorPositionIsValid)
                {
                    WaterGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;

            case ItemType.Chopping_tool:
                if(gridCursor.CursorPositionIsValid)
                {
                    ChopInPlayerDirection(gridPropertyDetails,itemDetails, playerDirection);
                }
                break;
            case ItemType.Collecting_tool:
                if(gridCursor.CursorPositionIsValid)
                {
                    CollectInPlayerDirection(gridPropertyDetails , itemDetails , playerDirection);
                }
                break;
            case ItemType.Reaping_tool:
                    if(cursor.CursorPositionIsValid)
                {
                    playerDirection = GetPlayerDirection(cursor.GetWorldPositionForCursor(), GetPlayerCenterPosition());
                    ReapInPlayerDirectionAtCursor(itemDetails , playerDirection);
                }
                break;

            case ItemType.Breaking_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    BreakInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            default:
                break;
        }
    }

   
    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if(itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputSeed( GridPropertyDetails gridPropertyDetails , ItemDetails itemDetails)
    {
        //�����ǰ grid �� gridPropertyDetails ��ʾ ��� grid ���ڹ� ����û���������� 
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid && gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.seedItemCode == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails , itemDetails );
        }

        else if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessCropWithEquippedItemInPlayerDirection(Vector3Int playerDirection, ItemDetails equipitemDetails, GridPropertyDetails gridPropertyDetails)
    {
        switch(equipitemDetails.itemType)
        {
            case ItemType.Chopping_tool:
            case ItemType.Breaking_tool:
                if (playerDirection == Vector3Int.right)
                {
                    isUsingToolRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isUsingToolLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isUsingToolUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isUsingToolDown = true;
                }
                break;
            case ItemType.Collecting_tool:
                if (playerDirection == Vector3Int.right)
                {
                    isPickingToolRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isPickingToolLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isPickingToolUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isPickingToolDown = true;
                }
                break;
            case ItemType.none:
                break;
        }

        Crop crop = GridPropertIesManager.Instance.GetCropObjectAtGridLocation(gridPropertyDetails);

        if(crop != null)
        {
            switch(equipitemDetails.itemType )
            {
                case ItemType.Chopping_tool:
                case ItemType.Breaking_tool:
                    crop.ProcessToolAction(equipitemDetails, isUsingToolRight, isUsingToolLeft, isUsingToolDown, isUsingToolUp);
                    break;

                case ItemType.Collecting_tool:
                    crop.ProcessToolAction(equipitemDetails , isPickingToolRight , isPickingToolLeft , isPickingToolDown , isPickingToolUp );
                    break;
            }
        }
    }

    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if (GridPropertIesManager.Instance.GetCropDetails(itemDetails.itemCode)!= null)
        {
            gridPropertyDetails.seedItemCode = itemDetails.itemCode;
            gridPropertyDetails.growthDays = 0;

            GridPropertIesManager.Instance.DisplayPlantedCrops(gridPropertyDetails);

            EventHandler.CallRemoveSelectedItemFromInventoryEvent();
        }
        
    }

    private void ResetAnimationTriggers()
    {
      //�������Զ�����״̬
     toolEffect = ToolEffect.none;
     isUsingToolRight = false ;        
     isUsingToolLeft = false;
     isUsingToolUp = false;
     isUsingToolDown = false;
     isLiftingToolRight = false;
     isLiftingToolLeft = false;
     isLiftingToolUp = false;
     isLiftingToolDown = false;
     isSwingingToolRight = false;
     isSwingingToolLeft = false;
     isSwingingToolUp = false;
     isSwingingToolDown = false;
     isPickingToolRight = false;
     isPickingToolLeft = false;
     isPickingToolUp = false;
     isPickingToolDown = false;
    }

    private void PlayerMovementInput()
    {
        _xInput = Input.GetAxisRaw("Horizontal"); 
        _yInput = Input.GetAxisRaw("Vertical");

        if(_yInput != 0 && _xInput != 0)
        {
            _xInput = _xInput * 0.71f;
            _yInput = _yInput * 0.71f;
        }

        if(_xInput != 0 || _yInput != 0)
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;

            if (_xInput < 0)
                playerDirection = Direction.Left;
            else if (_xInput > 0)
                playerDirection = Direction.Right;
            else if (_yInput < 0)
                playerDirection = Direction.Down;
            else 
                playerDirection = Direction.Up;
        }

        else if (_xInput == 0 || _yInput == 0)
        {
            isRunning = false;
            isWalking = false;
            isIdle = true;
        }    

    }

    private void PlayerWalkInput()
    {
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = false;
            isWalking = true;
            isIdle = false;
            movementSpeed = Settings.walkingSpeed;
        }
        else
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;
        }
    }

    public void ClearCarriedItem()
    {
        equippedItemSpriteRenderer.sprite = null;
        equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);

        armsCharacterAttribute.partVarianType = PartVarianType.none;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(armsCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        isCarrying = false;
    }

    public void ShowCarriedItem(int itemcode)
    {
        ItemDetails _itemDetails = InventoryManager.Instance.GetItemDetails(itemcode);

        if (_itemDetails != null) 
        {
            equippedItemSpriteRenderer.sprite = _itemDetails.itemSprite;
            equippedItemSpriteRenderer.color = new Color (1f,1f,1f, 1f);

            //Ӧ�� carry ���customisation  �� armsCharacterAttribute �� , �˻����ǰ��ֲ��Ķ������� ����Ϊ carry ״̬
            armsCharacterAttribute.partVarianType = PartVarianType.carry;
            characterAttributeCustomisationList.Clear();
            characterAttributeCustomisationList.Add(armsCharacterAttribute);

            animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

            isCarrying = true;
        }
    }

    

    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if(cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }
        else if (cursorGridPosition.x < playerGridPosition.x)
        {
            return Vector3Int.left;
        }

        else if (cursorGridPosition.y > playerGridPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerPosition)
    {
        if (cursorPosition.x > playerPosition.x 
            && cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2) 
            && cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2))
        {
            return Vector3Int.right;
        }
        else if (cursorPosition.x < playerPosition.x 
            && cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2) 
            && cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2))
        {
            return Vector3Int.left;
        }

        else if (cursorPosition.y > playerPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }


    public void DisablePlayerInputAndResetMovement()
    {
        DisablePlayerInput();
        ResetMovement();
        EventHandler.CallMovementEvent(_xInput, _yInput, toolEffect, isCarrying,
            isWalking, isRunning, isIdle,
         isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
         isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
         isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
         isPickingToolRight, isPickingToolLeft, isPickingToolUp, isPickingToolDown,
         false, false, false, false);
    

    }

    private void ResetMovement()
    {
        _xInput = 0f;
        _yInput = 0f;
        isRunning = false;
        isWalking = false;
        isIdle  = true;
    }

    public void DisablePlayerInput()
    {
        PlayerInputIsDisabled = true;
    }

    public void EnablePlayerInput()
    {
        PlayerInputIsDisabled = false;
    }


    ///ʱ��ϵͳ���� / ��������ϵͳ����
    private void PlayerTestInput()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TimeManager.Instance.Test_AdvanceGameMinute();
        }

        if (Input.GetKey(KeyCode.G))
        {
            TimeManager.Instance.Test_AdvanceGameDay();
        }
         
        if (Input.GetMouseButtonDown(1)) //����Ҽ�
        {
            ////�Ӷ������ȡ�� object �������� ������ڵ�λ����
            //GameObject tree = PoolManager.Instance.ReuseObject(canyonOakTreePrefab , mainCamera.ScreenToWorldPoint(
            //    new Vector3(Input.mousePosition.x , Input.mousePosition.y , -mainCamera.transform.position.z)) , Quaternion.identity);
            //tree.SetActive(true);
        }
    }

    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        StartCoroutine(HoeGroundAtCursorRoutine(playerDirection , gridPropertyDetails));
    }

    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        StartCoroutine(WaterGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }

    private void ChopInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equipitemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(ChopInPlayerDirectionRoutine(gridPropertyDetails, equipitemDetails, playerDirection));
    }


    private void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(CollectInPlayerDirectionRoutine(gridPropertyDetails, itemDetails , playerDirection ));
    }

    

    private void ReapInPlayerDirectionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(ReapInPlayerDirectionAtCursorRoutine(itemDetails, playerDirection));
    }

    private IEnumerator ReapInPlayerDirectionAtCursorRoutine(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //���� �����Ķ������� 
        toolCharacterAttribute.partVarianType = PartVarianType.scythe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        UseToolInPlayerDirection(itemDetails, playerDirection);

        yield return useToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void BreakInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(BreakInPlayerDirectionRoutine(gridPropertyDetails, itemDetails, playerDirection));
    }

    private IEnumerator BreakInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //���� ʮ�ָ�Ķ������� 
        toolCharacterAttribute.partVarianType = PartVarianType.pickaxe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, itemDetails, gridPropertyDetails);

        yield return useToolAnimationPause;

        yield return afterPickAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }


    private IEnumerator WaterGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //���ö���������ز���
        toolCharacterAttribute.partVarianType = PartVarianType.wateringCan;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);


        toolEffect = ToolEffect.watering;

        //�������õ���Щ ������� ������ update �� �� CallMovementEvent �ĵ��� �� ���ݸ� ���� ��ص� �� ���� �����Ĵ��� 
        if (playerDirection == Vector3Int.right)
        {
            isLiftingToolRight = true;
        }
        if (playerDirection == Vector3Int.up)
        {
            isLiftingToolUp = true;
        }
        if (playerDirection == Vector3Int.left)
        {
            isLiftingToolLeft = true;
        }
        if (playerDirection == Vector3Int.down)
        {
            isLiftingToolDown = true;
        }



        yield return liftToolAnimationPause; // ��ͣ startʱ  useToolAnimationPause �� ���õ�ʱ�� 

        // ����Ӧ�� grid ����Ϊ �� �ھ�� �� ״̬ 
        if (gridPropertyDetails.daySinceWatered == -1)
        {
            gridPropertyDetails.daySinceWatered = 0;
        }
        //���ö�Ӧ grid ��״̬ 
        GridPropertIesManager.Instance.SetGridPropertyDetials(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        GridPropertIesManager.Instance.DisplayWaterGround(gridPropertyDetails);
       

        yield return afterLiftToolAnimationPause;

        // �ָ� ��� ���� �� ʹ�ù��� 
        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private IEnumerator HoeGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //���ö���������ز���
        toolCharacterAttribute.partVarianType = PartVarianType.hoe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        //�������õ���Щ ������� ������ update �� �� CallMovementEvent �ĵ��� �� ���ݸ� ���� ��ص� �� ���� �����Ĵ��� 
        if (playerDirection == Vector3Int.right)
        {
            isUsingToolRight = true;
        }
        if (playerDirection == Vector3Int.up)
        {
            isUsingToolUp = true;
        }
        if (playerDirection == Vector3Int.left)
        {
            isUsingToolLeft = true;
        }
        if (playerDirection == Vector3Int.down)
        {
            isUsingToolDown = true;
        }



        yield return useToolAnimationPause; // ��ͣ startʱ  useToolAnimationPause �� ���õ�ʱ�� 

       // ����Ӧ�� grid ����Ϊ �� �ھ�� �� ״̬ 
       if(gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }
       //���ö�Ӧ grid ��״̬ 
        GridPropertIesManager.Instance.SetGridPropertyDetials(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        //����Ӧ grid ����ʾ �� sprite �滻 
        GridPropertIesManager.Instance.DisplayDugGround(gridPropertyDetails);

        yield return afterUseToolAnimationPause;

        // �ָ� ��� ���� �� ʹ�ù��� 
        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }


    private IEnumerator ChopInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equipitemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        toolCharacterAttribute.partVarianType = PartVarianType.axe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equipitemDetails, gridPropertyDetails);

        yield return useToolAnimationPause;

        yield return afterPickAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private IEnumerator CollectInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equipitemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equipitemDetails, gridPropertyDetails);

        yield return PickAnimationPause;

        yield return afterPickAnimationPause;

        PlayerInputIsDisabled = false ;
        playerToolUseDisabled = false;  
    }



    

    public Vector3 GetPlayerViewportPosition()
    {
        //�����������ͼ�е�λ�ã����� 00 �����½� 11 �����Ͻǣ�
        return mainCamera.WorldToViewportPoint(transform.position);
    }


    public Vector3 GetPlayerCenterPosition()
    {
        return new Vector3(transform.position.x , transform.position.y + Settings.playerCenterYOffset , transform.position.z);
    }


    private void UseToolInPlayerDirection(ItemDetails equippeditemDetails, Vector3Int playerDirection)
    {
        if(Input.GetMouseButtonDown(0))
        {
            switch (equippeditemDetails.itemType)
            {
                //������ҷ������� isSwingingTool �ķ���
                case ItemType.Reaping_tool:
                    if (playerDirection == Vector3Int.right)
                    {
                        isSwingingToolRight = true;
                    }

                    else if (playerDirection == Vector3Int.left)
                    {
                        isSwingingToolLeft = true;
                    }

                    else if (playerDirection == Vector3Int.up)
                    {
                        isSwingingToolUp = true;
                    }

                    else if (playerDirection == Vector3Int.down)
                    {
                        isSwingingToolDown = true;
                    }
                    break;
            }

            Vector2 point = new Vector2(GetPlayerCenterPosition().x + (playerDirection.x * (equippeditemDetails.itemUseRadius/2f)), 
                                        GetPlayerCenterPosition().y + playerDirection.y * (equippeditemDetails.itemUseRadius / 2f));

            Vector2 size = new Vector2(equippeditemDetails.itemUseRadius , equippeditemDetails .itemUseRadius );

            Item[] itemArray = HelperMethods.GetComponentsAtBoxLocationNonAlloc<Item>(Settings.maxCollidersToTestPerReapSwing, point, size, 0f);

            int reapableItemCount = 0;

            //�����ռ����� itemArray ����� Reapable_scenary �ʹݻ���
            for (int i = itemArray.Length - 1; i >= 0; i--)
            {
                if (itemArray[i] != null)
                {
                    if (InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType == ItemType.Reapable_scenary)
                    {
                        // ��Ч��λ�� 
                        Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x, itemArray[i].transform.position.y + Settings.gridCellSize / 2f
                            , itemArray[i].transform.position.z);

                        //������Ч 
                        EventHandler.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.reaping);

                        Destroy(itemArray[i].gameObject);

                        reapableItemCount++;
                        //���������� �������Χ�� ����Ŀ ��ǿ�Ʒ���
                        if(reapableItemCount >= Settings.maxTargetComponentsToDestroyPerReapSwing)
                            break;
                    }
                }
            }
        }
    }


    public GameObjectSave ISaveableSave()
    {
        //����Ѿ�׼�� �˳� ����� ��ǰ ��Ϸ�� savescene
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);
        //Ϊ��Ϸ�������µ� game object
        SceneSave sceneSave = new SceneSave();

        //����v3 �ֵ�
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();

        //����string (�������� ,��ǰ��������)
        sceneSave.stringDictionary = new Dictionary<string, string>();

        //�����λ�ü��뵽 v3 ���ֵ���
        Vector3Serializable vector3Serializable = new Vector3Serializable(transform.position.x,transform.position.y,transform.position.z);
        sceneSave.vector3Dictionary.Add("PlayerPosition",vector3Serializable);

        //��ӵ�ǰ�������� �� string �ֵ�
        sceneSave.stringDictionary.Add("CurrentScene", SceneManager.GetActiveScene().name);

        // �����ҵķ��� string �ֵ� , playerDirection ��һ��ö�� , ֱ�Ӹı� ��ɫ������ʾ,��������
        sceneSave.stringDictionary.Add("playerDirection",playerDirection.ToString());   

        // ��ӳ��� save ���� 
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;

    }

    void ISaveable.ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID,out GameObjectSave gameObjectSave))
        {
            if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                if (sceneSave.vector3Dictionary != null && sceneSave.vector3Dictionary.TryGetValue("PlayerPosition", out Vector3Serializable playerPosition))
                {
                    transform.position = new Vector3(playerPosition.x,playerPosition.y,playerPosition.z);
                }


                if (sceneSave.stringDictionary != null)
                {
                    //��ȡ������ڳ���
                    if (sceneSave.stringDictionary.TryGetValue("CurrentScene", out string currentScene))
                    {
                        SceneControllerManager.Instance.FadeAndLoadScene(currentScene, transform.position);
                    }
                    //��ȡ��ҷ���
                    if(sceneSave.stringDictionary.TryGetValue("playerDirection",out string playerDir))
                    {
                        bool playerDirFound = Enum.TryParse<Direction>(playerDir, true,out Direction direction);
                        if(playerDirFound)
                        {
                            playerDirection = direction;
                            SetPlayerDirection(playerDirection);
                        }
                    }
                }
            }
        }
    }

    private void SetPlayerDirection(Direction playerDirection)
    {
        switch (playerDirection)
        {
            case Direction.Up:

                EventHandler.CallMovementEvent(0f, 0f, ToolEffect.none, false,
                    false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false);

                break;
            case Direction.Down:
                EventHandler.CallMovementEvent(0f, 0f, ToolEffect.none, false,
                    false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false);
                break;
            case Direction.Left:
                EventHandler.CallMovementEvent(0f, 0f, ToolEffect.none, false,
                    false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false);
                break;
            case Direction.Right:
                EventHandler.CallMovementEvent(0f, 0f, ToolEffect.none, false,
                    false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false);
                break;
            default:
                EventHandler.CallMovementEvent(0f, 0f, ToolEffect.none, false,
                    false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false,
                    false, false, false, false);
                break;
        }
    }

    public void ISaveable_StoreScene(string sceneName)
    {
       //Nothing to do
    }

    public void ISaveable_RestoreScene(string sceneName)
    {
        //Nothing to do
    }

    public void ISaveable_Register()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveable_Deregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }





}



