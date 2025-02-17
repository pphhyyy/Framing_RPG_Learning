public enum InventoryLocation
{
    player,
    chest,//箱子
    count
}


public enum HarvestActionEffect
{ 
    deciduousLeavesFalling, //叶子掉落的特效
    pineConesFalling,
    choppingTreeTrunk,
    breakingStone,
    reaping,
    none
}

public enum ToolEffect
{
    none,
    watering
}

public enum Season
{
    Spring , 
    Summer , 
    Autumn , 
    Winter ,
    none ,
    count
}

public enum Direction
{ 
    Up, Down, Left, Right,none
}

public enum ItemType
{ 
    Seed,
    Commodity,
    Watering_tool,
    Hoeing_tool,
    Chopping_tool,
    Breaking_tool,
    Reaping_tool,
    Collecting_tool,
    Reapable_scenary,
    Furniture,
    none,
    count // 这里count 写在最后面，可以通过他的int 值 得知ItemType 这个枚举有多少项

}

public enum AnimationName
{
    idleDown ,
    idleUp ,
    idleRight ,
    idleLeft,

    walkDown,
    walkUp,
    walkRight,
    walkLeft,

    runDown,
    runUp,
    runRight,
    runLeft,

    useToolDown,
    useToolUp,
    useToolRight,
    useToolLeft,

    swingToolDown,
    swingToolUp,
    swingToolRight,
    swingToolLeft,

    liftToolDown,
    liftToolUp,
    liftToolRight,
    liftToolLeft,

    holdToolDown,
    holdToolUp,
    holdToolRight,
    holdToolLeft,

    pickDown,
    pickUp,
    pickRight,
    pickLeft,

    count  


}

public enum CharacterPartAnimator
{
    body,
    arms,
    hair,
    tool,
    hat,
    count
}

public enum PartVariantColor
{
    none,
    count,
}

public enum PartVarianType
{
    none,
    carry,
    hoe,
    pickaxe,
    axe,
    scythe,
    wateringCan,
    count
}

public enum SceneName
{
    scene1_Farm,
    scene2_Field, 
    scene3_Cabin,

}

public enum GridBoolProperty
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle
}