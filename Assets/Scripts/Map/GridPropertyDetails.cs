[System.Serializable]
public class GridPropertyDetails 
{
    public int gridX;
    public int gridY;

    public bool isDiggable = false;
    public bool canDropItem = false;
    public bool canPlaceFurniture = false;
    public bool isPath = false;
    public bool isNPCObstacle = false;

    //记录这个 网格上 的一些 属性 ， 因为要配合之后的时间系统 耕种系统 这里要记录自从
    public int daysSinceDug = -1; //从上一次挖掘到现在多久 
    public int daySinceWatered = -1;
    public int seedItemCode = -1;
    public int growthDays = -1;
    public int daysSinceLastHarvest = -1; //从上一次收获到现在多久 

    public GridPropertyDetails() { }

}
