[System.Serializable]
//�������� , ������ ���� Bool ���ԣ���dig���� �ɷ��üҾ� �ɷ�����Ʒ�� �� �Ͷ�Ӧ���Ե�bool ֵ 
public class GridProperty 
{
    public GridCoordinate gridCoordinate;
    public GridBoolProperty gridBoolProperty;
    public bool gridBoolValue = false;

    public GridProperty(GridCoordinate gridCoordinate, GridBoolProperty gridBoolProperty, bool gridBoolValue)
    {
        this.gridCoordinate = gridCoordinate;
        this.gridBoolProperty = gridBoolProperty;
        this.gridBoolValue = gridBoolValue;
    }
}
