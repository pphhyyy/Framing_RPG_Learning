public enum InventoryLocation
{
    player,
    chest,//����
    count
}


public enum HarvestActionEffect
{ 
    deciduousLeavesFalling, //Ҷ�ӵ������Ч
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
    count // ����count д������棬����ͨ������int ֵ ��֪ItemType ���ö���ж�����

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