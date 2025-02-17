using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class Player : SingletonMonobehaviour<Player>, ISaveable
{

    //WaitForSeconds 暂停协程 一段
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
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null; //就是玩家选中物品栏中的某个item 后，需要举起来的那个物体的 sprite 

    private CharacterAttribute armsCharacterAttribute;
    private CharacterAttribute toolCharacterAttribute;

    private bool _playerInputIsDisabled = false;
    public bool PlayerInputIsDisabled { get => _playerInputIsDisabled; set => _playerInputIsDisabled = value; }

    // Isaveable 接口要求的属性
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

        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.arms , PartVariantColor.none , PartVarianType.none ); //手部的角色特性，和后面的动画覆盖有关
        toolCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.tool, PartVariantColor.none, PartVarianType.none); //手部的角色特性，和后面的动画覆盖有关


        characterAttributeCustomisationList = new List<CharacterAttribute>();

        //从脚本的 unique id 生成器上,获取当前脚本的Unique ID

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
        
        mainCamera =  Camera.main; //这里获取的mian camera 必须是标签为 maincamera 的 camera 
    }

    private void OnEnable()
    {
        //注册 保存 
        ISaveable_Register();

        //就是场景过渡时 应该关闭 输入和移动 
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

        Debug.Log("来自mac air 的远程调试");
        //用 Settings 中设置好的值 填充 这里的 两个 WaitForSeconds ， 这里两个暂停 是为了优化 使用 工具时 动画 的显示 效果 
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

            PlayerClickInput(); //就是不拖动，直接选中然后点击 触发的事件

            PlayerTestInput();

            
            
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
                    //获取 当 光标 和 玩家 的位置， 
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

        // 玩家 应该 面向 光标所在的方向 
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
        //如果当前 grid 的 gridPropertyDetails 显示 这个 grid 被挖过 而且没有种下种子 
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
      //重置所以动画的状态
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

            //应用 carry 这个customisation  到 armsCharacterAttribute 上 , 人话就是把手部的动画属性 设置为 carry 状态
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


    ///时间系统测试 / 场景加载系统测试
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
         
        if (Input.GetMouseButtonDown(1)) //鼠标右键
        {
            ////从对象池中取出 object 并放置在 鼠标所在的位置上
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

        //设置 镰刀的动画覆盖 
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

        //设置 十字镐的动画覆盖 
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

        //设置动画覆盖相关参数
        toolCharacterAttribute.partVarianType = PartVarianType.wateringCan;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);


        toolEffect = ToolEffect.watering;

        //下面设置的这些 方向参数 会随着 update 中 对 CallMovementEvent 的调用 ， 传递给 动画 相关的 类 进行 动画的处理 
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



        yield return liftToolAnimationPause; // 暂停 start时  useToolAnimationPause 中 设置的时间 

        // 将对应的 grid 设置为 被 挖掘过 的 状态 
        if (gridPropertyDetails.daySinceWatered == -1)
        {
            gridPropertyDetails.daySinceWatered = 0;
        }
        //设置对应 grid 的状态 
        GridPropertIesManager.Instance.SetGridPropertyDetials(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        GridPropertIesManager.Instance.DisplayWaterGround(gridPropertyDetails);
       

        yield return afterLiftToolAnimationPause;

        // 恢复 玩家 输入 和 使用工具 
        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private IEnumerator HoeGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //设置动画覆盖相关参数
        toolCharacterAttribute.partVarianType = PartVarianType.hoe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        //下面设置的这些 方向参数 会随着 update 中 对 CallMovementEvent 的调用 ， 传递给 动画 相关的 类 进行 动画的处理 
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



        yield return useToolAnimationPause; // 暂停 start时  useToolAnimationPause 中 设置的时间 

       // 将对应的 grid 设置为 被 挖掘过 的 状态 
       if(gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }
       //设置对应 grid 的状态 
        GridPropertIesManager.Instance.SetGridPropertyDetials(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        //将对应 grid 上显示 的 sprite 替换 
        GridPropertIesManager.Instance.DisplayDugGround(gridPropertyDetails);

        yield return afterUseToolAnimationPause;

        // 恢复 玩家 输入 和 使用工具 
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
        //返回玩家在试图中的位置（这里 00 是左下角 11 是右上角）
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
                //根据玩家方向，设置 isSwingingTool 的方向
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

            //遍历收集到的 itemArray 如果有 Reapable_scenary 就摧毁它
            for (int i = itemArray.Length - 1; i >= 0; i--)
            {
                if (itemArray[i] != null)
                {
                    if (InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType == ItemType.Reapable_scenary)
                    {
                        // 特效的位置 
                        Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x, itemArray[i].transform.position.y + Settings.gridCellSize / 2f
                            , itemArray[i].transform.position.z);

                        //触发特效 
                        EventHandler.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.reaping);

                        Destroy(itemArray[i].gameObject);

                        reapableItemCount++;
                        //超出最大可以 割掉（范围） 的数目 就强制返回
                        if(reapableItemCount >= Settings.maxTargetComponentsToDestroyPerReapSwing)
                            break;
                    }
                }
            }
        }
    }


    public GameObjectSave ISaveableSave()
    {
        //如果已经准备 退出 就清除 当前 游戏的 savescene
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);
        //为游戏对象建立新的 game object
        SceneSave sceneSave = new SceneSave();

        //建立v3 字典
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();

        //建立string (数据类型 ,当前场景名字)
        sceneSave.stringDictionary = new Dictionary<string, string>();

        //将玩家位置加入到 v3 的字典中
        Vector3Serializable vector3Serializable = new Vector3Serializable(transform.position.x,transform.position.y,transform.position.z);
        sceneSave.vector3Dictionary.Add("PlayerPosition",vector3Serializable);

        //添加当前场景名字 到 string 字典
        sceneSave.stringDictionary.Add("CurrentScene", SceneManager.GetActiveScene().name);

        // 添加玩家的方向到 string 字典 , playerDirection 是一个枚举 , 直接改变 角色动画显示,不是向量
        sceneSave.stringDictionary.Add("playerDirection",playerDirection.ToString());   

        // 添加场景 save 数据 
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
                    //获取玩家所在场景
                    if (sceneSave.stringDictionary.TryGetValue("CurrentScene", out string currentScene))
                    {
                        SceneControllerManager.Instance.FadeAndLoadScene(currentScene, transform.position);
                    }
                    //获取玩家方向
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



