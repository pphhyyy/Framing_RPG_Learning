using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private int _itemUseGridRadius = 0;
    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; }

    private ItemType _selectedItemType = 0;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    private bool _cursorIsEnable = false;
    public bool CursorIsEnable { get => _cursorIsEnable; set => _cursorIsEnable = value; }

    private void OnDisable()
    {
        EventHandler.AfterSceneloadEvent -= SceneLoaded;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneloadEvent += SceneLoaded;
    }

    private void SceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if(CursorIsEnable)
        {
            DisplayCursor();
        }
    }

    private Vector3Int DisplayCursor()
    {
        if(grid != null)
        {
            Vector3Int gridPosition = GetGridPositionForCursor();

            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            //Debug.Log("gridPosition: " + gridPosition + "====playerGridPosition: " +playerGridPosition) ;

            SetCursorValidity(gridPosition, playerGridPosition); // ���ù�����Ч��Χ

            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        }

        else
        {
            return Vector3Int.zero;
        }
    }

 
    private void SetCursorValidity(Vector3Int gridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();

        if(Mathf.Abs(gridPosition.x - playerGridPosition.x) > ItemUseGridRadius || Mathf.Abs(gridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            //Debug.Log("�������");
            //��ǰ���λ�ú����λ�ò����� ��Ч���İ뾶������Ϊ��Ч
            SetCursorToInvalid(); // ���������Ϊ��Ч
            return;
        }

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);
        if(itemDetails == null) // �����ǰû��ѡ��item ˵����Ч 
        {
            //Debug.Log("û��ѡ��");
            SetCursorToInvalid();
            return;
        }
        //�ӹ���λ�ã���ȡ��Ӧgridmap �� �� grid ����ϸ����
        GridPropertyDetails gridPropertyDetails = GridPropertIesManager.Instance.GetGridPropertyDetails(gridPosition.x, gridPosition.y);

        if (gridPropertyDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if(!IsCursorValidForSeed(gridPropertyDetails))
                    {

                        //Debug.Log("seed �޷������ڴ�");
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.Commodity:
                    if (!IsCursorValidForCommodity(gridPropertyDetails))
                    {
                        //Debug.Log("Commodity �޷������ڴ�");
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.Watering_tool: // ��������д�� ��˼���� ���� Watering_tool �� Hoeing_tool ��ִ�� ����Ĵ���
                case ItemType.Hoeing_tool:
                case ItemType.Chopping_tool:
                case ItemType.Breaking_tool:
                case ItemType.Reaping_tool:
                case ItemType.Collecting_tool:
                    if (!IsCursorValidForTool(gridPropertyDetails , itemDetails))
                    {
                        //Debug.Log("�˴��޷�ʹ�ù���");
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.none:
                    break;
                case ItemType.count:
                    break;

                    default:
                    break;
            }
        }
        else
        {
            Debug.Log("�� grid ��Ч");
            SetCursorToInvalid();
            return;
        }
    }

    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        switch(itemDetails.itemType)
        {
            case ItemType.Hoeing_tool : // �����ǰ�õ��� ��ͷ 
                if(gridPropertyDetails.isDiggable == true && gridPropertyDetails.daysSinceDug == -1) // �ҵ�ǰgrid ���� �� ��û�б� �ڹ� 
                {
                    #region ��Ҫ��鵱ǰλ���ϵ� item �Ƿ��� reapable �� , ���������ǰλ�����вݣ���ô����ڵص�ʱ��Ҳ��Ѳ�һͬ�ڵ�
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                    List<Item> itemList = new List<Item>();
                    HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSize, 0f); //��õ�ǰ grid �� ȫ���� item �� װ�� itemList

                    #endregion

                    bool foundReapable = false;
                    foreach(Item item in itemList)
                    {
                        // ��� ��ǰ grid �� �ҵ��� item �� ��û�� type ��  Reapable_scenary �� 
                        if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                        {
                            foundReapable = true;
                            break;
                        }

                        
                    }
                    //��鵱ǰ grid ����û�� crop �� crop �Ͳ�����
                    CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

                    if(cropDetails != null) // ������� seedItemCode �� so_CropDetailsList ���ҵ��˶�Ӧ�� crop 
                    {
                        return false;
                    }


                    if (foundReapable) // �����ǰ grid �� �� Reapable_scenary �� item ˵�� ��� grid ������ ���� false
                        { return false; }
                    else { return true; }
                }

                else 
                    return false;

            case ItemType.Watering_tool: // �����ǰ�õ��� ���� 
                if (gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.daySinceWatered == -1) // �ҵ�ǰgrid �� �� �����һ�û����ˮ 
                {
                    return true;
                }

                else
                    return false;
            case ItemType.Chopping_tool:
            case ItemType.Collecting_tool:
            case ItemType.Breaking_tool:
                //��鵱ǰ�� crop��ͨ�� gridPropertyDetails �õ��� �ܷ� �� Ŀǰѡ�е� tool ���ɼ� �� ͬʱ��� ��� item��crop�� �Ƿ���ȫ������

                //��ǰ grid ���Ƿ�����������
                if (gridPropertyDetails.seedItemCode != -1 )
                {
                    //ͨ�� seedItemCode ��õ�ǰ crop �� detail 
                    CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

                    if(cropDetails != null) // ������� seedItemCode �� so_CropDetailsList ���ҵ��˶�Ӧ�� crop 
                    {
                        // �����ǰ grid �����µ� crop �������ڴ��ڶ�Ӧ crop ��Ҫ�����������(���� growthDay ���� �����һ�� )��˵����� crop �Ѿ�������
                        if (gridPropertyDetails.growthDays >= cropDetails.growthDay[cropDetails.growthDay.Length - 1 ]) 
                            
                        {
                            //��� crop ���ù��߲�ժ��
                            if(cropDetails.CanUseToolToHarvestCrop(itemDetails.itemCode))
                            {
                                return true ;
                            }
                            else
                            { return false; }
                        }
                        else
                            return false;
                    }
                    
                }
                return false;
            default:
                return false;
                
        }
    }

   

    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem; // �����ǰ�ĸ��� canDropItem Ϊ�� ˵��������Ӷ� IsCursorValidForCommodity ��Ч ����ֱ�ӷ��� canDropItem ����
    }

    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    public void DisableCursor()
    {
        cursorImage.color = Color.clear;
        CursorIsEnable = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnable = true;
    }

    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }


    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(Player.Instance.transform.position);
    }

    public Vector3Int GetGridPositionForCursor()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        
        return grid.WorldToCell(worldPosition);
    }

    private Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition); //�������굽��Ļ����
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }

    private Vector3 GetWorldPositionForCursor()
    {
        return grid.CellToWorld(GetGridPositionForCursor());
    }

}
